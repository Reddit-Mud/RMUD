using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandParser
    {
		public class CommandEntry
		{
			internal ICommandTokenMatcher Matcher;
            internal List<String> ScoreArguments;
			internal ICommandProcessor Processor;
			internal String HelpText;
		}

		internal List<CommandEntry> Commands = new List<CommandEntry>();

        public void AddCommand(ICommandTokenMatcher Matcher, ICommandProcessor Processor, String HelpText, params String[] ScoreArguments)
        {
            var Entry = new CommandEntry { Matcher = Matcher, Processor = Processor, HelpText = HelpText, ScoreArguments = new List<string>(ScoreArguments) };
			Commands.Add(Entry);
        }

		public class MatchedCommand
		{
			public CommandEntry Command;
			public List<PossibleMatch> Matches;

			public MatchedCommand(CommandEntry Command, IEnumerable<PossibleMatch> Matches)
			{
				this.Command = Command;
                if (Matches != null)
                    this.Matches = new List<PossibleMatch>(Matches);
                else
                    this.Matches = new List<PossibleMatch>();
			}
		}

        public class MatchAborted : Exception
        {
            public MatchAborted(String Message) : base(Message) { }
        }

		public class MatchContext
		{
			public Actor ExecutingActor;

			private List<MatchableObject> CachedObjectsInScope = null;
			public List<MatchableObject> ObjectsInScope
			{
				get
				{
					if (CachedObjectsInScope != null) return CachedObjectsInScope;
					CachedObjectsInScope = new List<MatchableObject>();

					var location = ExecutingActor.Location as Room;
					if (location != null)
					{
                        location.EnumerateObjects(RelativeLocations.Everything, (o, l) =>
                            {
                                if (o is IMatchable)
                                    CachedObjectsInScope.Add(new MatchableObject(o as IMatchable, l));
                                return EnumerateObjectsControl.Continue;
                            });
                    }
	
					return CachedObjectsInScope;
				}
			}
		}

        internal MatchedCommand ParseCommand(String Command, Actor Actor)
        {
			var tokens = new LinkedList<String>(Command.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
			var rootMatch = new PossibleMatch(tokens.First);

			//Find all objects in scope
			var matchContext = new MatchContext { ExecutingActor = Actor };

			foreach (var command in Commands)
			{
                IEnumerable<PossibleMatch> matches;

                try
                {
                    matches = command.Matcher.Match(rootMatch, matchContext);
                }
                catch (MatchAborted ma)
                {
                    return new MatchedCommand(new CommandEntry { Processor = new Commands.ReportError(ma.Message) }, new PossibleMatch[] { new PossibleMatch(new Dictionary<string,object>(), null) });
                }

                //Only accept matches that consumed all of the input.
                matches = matches.Where(m => m.Next == null);

                //If we did, however, consume all of the input, we will assume this match is successful.
                if (matches.Count() > 0)
                {
                    if (command.ScoreArguments != null)
                    {
                        foreach (var scoreArgumentName in command.ScoreArguments)
                        {
                            var highestScoreFound = Int32.MinValue;
                            foreach (var match in matches)
                            {
                                var score = GetScore(match, scoreArgumentName);
                                if (score > highestScoreFound) highestScoreFound = score;
                            }
                            matches = matches.Where(m => highestScoreFound == GetScore(m, scoreArgumentName));
                        }
                    }

                    System.Diagnostics.Debug.Assert(matches.Count() > 0);
                    return new MatchedCommand(command, matches);
                }
			}
            return null;
        }

        private static int GetScore(PossibleMatch Match, String ScoreArgumentName)
        {
            if (Match.Arguments.ContainsKey(ScoreArgumentName))
            {
                var argScore = Match.Arguments[ScoreArgumentName] as int?;
                if (argScore.HasValue) return argScore.Value;
            }

            return 0; //If there is no score, the match is neutral.
        }
    }
}

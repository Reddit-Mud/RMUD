using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandParser
    {

		internal List<CommandEntry> Commands = new List<CommandEntry>();

        public CommandEntry AddCommand(CommandTokenMatcher Matcher)
        {
            var Entry = new CommandEntry { Matcher = Matcher };
            Entry.ManualName = Matcher.FindFirstKeyWord();
            Commands.Add(Entry);
            return Entry;
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

        internal MatchedCommand ParseCommand(String Command, Actor Actor)
        {
			var tokens = new LinkedList<String>(Command.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
			var rootMatch = new PossibleMatch(tokens.First);
            rootMatch.Upsert("ACTOR", Actor);

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
                    return new MatchedCommand(
                        new CommandEntry().ProceduralRule((match, actor) => 
                        {
                            Mud.SendMessage(actor, ma.Message);
                            return PerformResult.Continue;
                        }), 
                        new PossibleMatch[] { new PossibleMatch(null) });
                }

                //Only accept matches that consumed all of the input.
                matches = matches.Where(m => m.Next == null);

                //If we did, however, consume all of the input, we will assume this match is successful.
                if (matches.Count() > 0)
                    return new MatchedCommand(command, matches);
            }
            return null;
        }
    }
}

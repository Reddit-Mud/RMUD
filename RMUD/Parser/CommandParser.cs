using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandParser
    {
		internal class CommandEntry
		{
			internal ICommandTokenMatcher Matcher;
			internal ICommandProcessor Processor;
			internal String HelpText;
		}

		internal List<CommandEntry> Commands = new List<CommandEntry>();

        public void AddCommand(ICommandTokenMatcher Matcher, ICommandProcessor Processor, String HelpText)
        {
            var Entry = new CommandEntry { Matcher = Matcher, Processor = Processor, HelpText = HelpText };
			Commands.Add(Entry);
        }

		internal class MatchedCommand
		{
			public CommandEntry Command;
			public List<PossibleMatch> Matches;

			public MatchedCommand(CommandEntry Command, IEnumerable<PossibleMatch> Matches)
			{
				this.Command = Command;
				this.Matches = new List<PossibleMatch>(Matches);
			}
		}

		public class MatchContext
		{
			public Actor ExecutingActor;

			private List<IMatchable> CachedObjectsInScope = null;
			public List<IMatchable> ObjectsInScope
			{
				get
				{
					if (CachedObjectsInScope != null) return CachedObjectsInScope;
					CachedObjectsInScope = new List<IMatchable>();

					CachedObjectsInScope.AddRange(ExecutingActor.Inventory);

					var location = ExecutingActor.Location as Room;
					if (location != null)
					{
						CachedObjectsInScope.AddRange(location.Contents);
						CachedObjectsInScope.AddRange(location.Links.Where(l => (l.Door != null && l.Door is IMatchable)).Select(l => l.Door as IMatchable));
						CachedObjectsInScope.AddRange(location.Scenery);
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
                //Only accept matches that consumed all of the input.
                var matches = command.Matcher.Match(rootMatch, matchContext).Where(m => m.Next == null);

                //If we did, however, consume all of the input, we will assume this match is successful.
                if (matches.Count() > 0)
                    return new MatchedCommand(command, matches);
			}
            return null;
        }

		internal List<MatchedCommand> FindAllGoodMatches(String Command, Actor Actor)
		{
			var tokens = new LinkedList<String>(Command.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
			var rootMatch = new PossibleMatch(tokens.First);

			var matchContext = new MatchContext { ExecutingActor = Actor };

			var r = new List<MatchedCommand>();

			foreach (var command in Commands)
			{
				var matches = command.Matcher.Match(rootMatch, matchContext);
                r.Add(new MatchedCommand(command, matches));
			}

			return r;
		}
    }
}

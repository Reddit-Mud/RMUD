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
		}

		internal List<CommandEntry> Commands = new List<CommandEntry>();

        public void AddCommand(ICommandTokenMatcher Matcher, ICommandProcessor Processor)
        {
            var Entry = new CommandEntry { Matcher = Matcher, Processor = Processor };
			Commands.Add(Entry);
        }

		internal class MatchedCommand
		{
			public CommandEntry Command;
			public PossibleMatch Match;

			public MatchedCommand(CommandEntry Command, PossibleMatch Match)
			{
				this.Command = Command;
				this.Match = Match;
			}
		}

		public class MatchContext
		{
			public Actor ExecutingActor;

			private List<Thing> CachedObjectsInScope = null;
			public List<Thing> ObjectsInScope
			{
				get
				{
					if (CachedObjectsInScope != null) return CachedObjectsInScope;
					CachedObjectsInScope = new List<Thing>();
					var location = ExecutingActor.Location as Room;
					if (location != null)
						CachedObjectsInScope.AddRange(location.Contents);
						
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
				var matches = command.Matcher.Match(rootMatch, matchContext);
				var firstGoodMatch = matches.Find(m => m.Next == null);
				if (firstGoodMatch != null) return new MatchedCommand(command, firstGoodMatch);
			}
            return null;
        }
        
    }
}

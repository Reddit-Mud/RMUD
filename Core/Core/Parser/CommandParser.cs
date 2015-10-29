using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    /// <summary>
    /// Implements the process of matching the player's input to a command.
    /// </summary>
    public partial class CommandParser
    {
		internal List<CommandEntry> Commands = new List<CommandEntry>();
        internal String ModuleBeingInitialized = null;

        public CommandEntry AddCommand(CommandTokenMatcher Matcher)
        {
            var Entry = new CommandEntry { Matcher = Matcher };
            Entry.SourceModule = ModuleBeingInitialized;
            Entry.ManualName = Matcher.FindFirstKeyWord();
            Commands.Add(Entry);
            return Entry;
        }

        public CommandEntry FindCommandWithID(String ID)
        {
            return Commands.FirstOrDefault(c => c._ID == ID);
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

        public MatchedCommand ParseCommand(PendingCommand Command)
        {
            var tokens = new LinkedList<String>(Command.RawCommand.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));

			var rootMatch = new PossibleMatch(tokens.First);
            rootMatch.Upsert("ACTOR", Command.Actor);
            rootMatch.Upsert("LOCATION", Command.Actor == null ? null : Command.Actor.Location);

            // A pending command can have some properties set when it is queued. For example, the 'go' action
            // generates a 'look' command after processing. It will give the 'look' command a property named
            // 'auto' to indicate that the look command was generated. Rules can then use this property to
            // implement behavior. From the same example, 'look' may emit a brief description if it is generated
            // by the 'go' action, but emit a more detailed description if the player enters the command 'look'.
            //      See StandardActions.Go
            if (Command.PreSettings != null)
                foreach (var setting in Command.PreSettings)
                    rootMatch.Upsert(setting.Key.ToUpper(), setting.Value);

			var matchContext = new MatchContext { ExecutingActor = Command.Actor };

            // Try every single command defined, until one matches.
            foreach (var command in Commands)
            {
                IEnumerable<PossibleMatch> matches;

                try
                {
                    matches = command.Matcher.Match(rootMatch, matchContext);
                }
                catch (MatchAborted ma)
                {
                    // The match didn't fail; it generated an error. These means the match progressed to a point 
                    // where the author of the command felt that the input could not logically match any other
                    // command, however, the input was still malformed in some way. Abort matching, and dummy up
                    // a command entry to display the error message to the player.
                    return new MatchedCommand( 
                        new CommandEntry().ProceduralRule((match, actor) => 
                        {
                            MudObject.SendMessage(actor, ma.Message);
                            return SharpRuleEngine.PerformResult.Continue;
                        }), 
                        // We need a fake match just so it can be passed to the procedural rule.
                        new PossibleMatch[] { new PossibleMatch(null) });
                }

                // Only accept matches that consumed all of the input.
                matches = matches.Where(m => m.Next == null);

                // If we did consume all of the input, we will assume this match is successful. Note it is 
                // possible for a command to generate multiple matches, but not possible to match multiple commands.
                if (matches.Count() > 0)
                    return new MatchedCommand(command, matches);
            }
            return null;
        }
    }
}

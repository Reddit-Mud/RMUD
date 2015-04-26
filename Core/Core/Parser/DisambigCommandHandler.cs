using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
    /// <summary>
    /// A client command handler that allows the player to disambiquate between multiple choices.
    /// </summary>
	public class DisambigCommandHandler : ClientCommandHandler
	{
		public ParserCommandHandler ParentHandler;
        public CommandParser.MatchedCommand MatchedCommand;
        public String DisambigArgument = null;
        public List<MudObject> DisambigObjects = null;

		public DisambigCommandHandler(
            Actor Actor, 
            CommandParser.MatchedCommand MatchedCommand, 
            ParserCommandHandler ParentHandler)
		{
            this.ParentHandler = ParentHandler;
            this.MatchedCommand = MatchedCommand;

            // Find an object parameter such that 
            //  a) each match has a parameter with that name 
            //  b) at least one match has a value different from the others

            var foundAmbiguousArgument = false;

            // Note that we iterate only over the arguments in the first match. If the first match doesn't have the
            // argument, that argument can't satisfy condition a above.
            foreach (var argument in MatchedCommand.Matches[0])
            {
                // It's only possible to disambiguate on MudObjects. If any match - including the first one - has 
                // a value for that argument that is not a MudObject, disambiguation will fail.
                if (argument.Value is MudObject)
                {
                    var uniqueMatchables = new List<MudObject>();
                    var rejected = false;

                    foreach (var match in MatchedCommand.Matches)
                    {
                        if (match.ContainsKey(argument.Key) &&
                            match[argument.Key] is MudObject)
                        {
                            var matchableObject = match[argument.Key] as MudObject;
                            if (!uniqueMatchables.Contains(matchableObject))
                                uniqueMatchables.Add(matchableObject);
                        }
                        else
                        {
                            rejected = true;
                            break;
                        }
                    }

                    if (!rejected && uniqueMatchables.Count > 1)
                    {
                        // Disambiguate on this object.
                        DisambigArgument = argument.Key;
                        DisambigObjects = uniqueMatchables;

                        foundAmbiguousArgument = true;
                        break;
                    }
                }
            }

            if (foundAmbiguousArgument)
            {
                var response = new StringBuilder();
                response.Append("Which did you mean?\r\n");
                for (var i = 0; i < DisambigObjects.Count; ++i)
                    response.Append(String.Format("{0}: {1}\r\n", i, Core.GlobalRules.ConsiderValueRule<String>("printed name", Actor, DisambigObjects[i], "the")));
                MudObject.SendMessage(Actor, response.ToString());
            }
            else
            {
                MudObject.SendMessage(Actor, "I couldn't figure out how to disambiguate that command.");
            }
		}

        public void HandleCommand(PendingCommand Command)
        {
            Command.Actor.CommandHandler = ParentHandler;
            
            //Just retry if the attempt to help has failed.
            if (DisambigObjects == null)
            {
                Core.EnqueuActorCommand(Command);
                return;
            }
            
            int ordinal = 0;
            if (Int32.TryParse(Command.RawCommand, out ordinal))
            {
                if (ordinal < 0 || ordinal >= DisambigObjects.Count)
                    MudObject.SendMessage(Command.Actor, "That wasn't a valid option. I'm aborting disambiguation.");
                else
                {
                    var choosenMatches = MatchedCommand.Matches.Where(m => Object.ReferenceEquals(m[DisambigArgument], DisambigObjects[ordinal]));
                    MatchedCommand.Matches = new List<PossibleMatch>(choosenMatches);

                    if (MatchedCommand.Matches.Count == 1)
                        Core.ProcessPlayerCommand(MatchedCommand.Command, MatchedCommand.Matches[0], Command.Actor);
                    else
                    {
                        // WHat? There are still multiple options?
                        MudObject.SendMessage(Command.Actor, "That helped narrow it down, but I'm still not sure what you mean.");
                        Command.Actor.CommandHandler = new DisambigCommandHandler(Command.Actor, MatchedCommand, ParentHandler);
                    }
                }
            }
            else 
            {
                // The input was nor an ordial. Go ahead and requeue the command so the normal command handler
                // can take care of it.
                Core.EnqueuActorCommand(Command);
            }
        }
    }
}

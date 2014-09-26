using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class DisambigCommandHandler : IClientCommandHandler
	{
		public ParserCommandHandler ParentHandler;
        public CommandParser.MatchedCommand MatchedCommand;
        public String DisambigArgument = null;
        public List<Thing> DisambigObjects = null;

		public DisambigCommandHandler(
            Client Client, 
            CommandParser.MatchedCommand MatchedCommand, 
            ParserCommandHandler ParentHandler)
		{
            this.ParentHandler = ParentHandler;
            this.MatchedCommand = MatchedCommand;

            //Find an object parameter such that 
            //  a) each match has a parameter with that name 
            //  b) at least one match has value different from the others

            var foundAmbiguousArgument = false;

            foreach (var argument in MatchedCommand.Matches[0].Arguments)
            {
                if (argument.Value is Thing)
                {
                    var uniqueMatchables = new List<Thing>();
                    var rejected = false;

                    foreach (var match in MatchedCommand.Matches)
                    {
                        if (match.Arguments.ContainsKey(argument.Key) &&
                            match.Arguments[argument.Key] is Thing)
                        {
                            var matchableObject = match.Arguments[argument.Key] as Thing;
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
                        //Disambiguate on this object.
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
                    response.Append(String.Format("{0}: {1}\r\n", i, DisambigObjects[i].Definite));
                Mud.SendMessage(Client, response.ToString());
            }
            else
            {
                Mud.SendMessage(Client, "That command is too ambiguous for me to help you with.");
            }
		}

        public void HandleCommand(Client Client, String Command)
        {
            Client.CommandHandler = ParentHandler;
            
            //Just retry if the attempt to help has failed.
            if (DisambigObjects == null)
            {
                Mud.EnqueuClientCommand(Client, Command);
                return;
            }
            
            int ordinal = 0;
            if (Int32.TryParse(Command, out ordinal))
            {
                if (ordinal < 0 || ordinal >= DisambigObjects.Count)
                    Mud.SendMessage(Client, "That wasn't a valid option. I'm aborting disambiguation.\r\n");
                else
                {
                    var choosenMatches = MatchedCommand.Matches.Where(m => Object.ReferenceEquals(m.Arguments[DisambigArgument], DisambigObjects[ordinal]));
                    MatchedCommand.Matches = new List<PossibleMatch>(choosenMatches);

                    if (MatchedCommand.Matches.Count == 1)
                        MatchedCommand.Command.Processor.Perform(MatchedCommand.Matches[0], Client.Player);
                    else
                    {
                        Mud.SendMessage(Client, "That helped narrow it down, but I'm still not sure what you mean.\r\n");
                        Client.CommandHandler = new DisambigCommandHandler(Client, MatchedCommand, ParentHandler);
                    }
                }
            }
            else //Player didn't type an ordinal; retry.
            {
                Mud.EnqueuClientCommand(Client, Command);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Errors : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
               new Sequence(
                   new KeyWord("ERRORS", false),
                   new Optional(
                       new Number("COUNT"))),
                new ErrorsProcessor(),
                "Display the error log.");
        }
    }

    internal class ErrorsProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            int count = 20;
            if (Match.Arguments.ContainsKey("COUNT")) count = (Match.Arguments["COUNT"] as int?).Value;

            var logFilename = "errors.log";
            if (System.IO.File.Exists(logFilename))
            {
                foreach (var line in (new ReverseLineReader(logFilename)).Take(count).Reverse())
                    Mud.SendMessage(Actor, line);
            }
            else
            {
                Mud.SendMessage(Actor, "Error log does not exist.");
            }
        }
    }
}
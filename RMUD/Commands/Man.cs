using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Man : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("MAN"),
                        KeyWord("?")),
                    Optional(Rest("COMMAND"))),
                "Display a list of all defined commands.")
                .Manual("This is the command you typed to get this message.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.Arguments.ContainsKey("COMMAND"))
                    {
                        Mud.SendMessage(actor, "Available help topics");
                        var line = "";
                        foreach (var manPage in Mud.ManPages.Select(p => p.Name).Distinct().OrderBy(s => s))
                        {
                            line += manPage;
                            if (line.Length < 20) line += new String(' ', 20 - line.Length);
                            else if (line.Length < 40) line += new String(' ', 40 - line.Length);
                            else
                            {
                                Mud.SendMessage(actor, line);
                                line = "";
                            }
                        }
                    }
                    else
                    {
                        var manPageName = match.Arguments["COMMAND"].ToString().ToUpper();
                        var pages = new List<ManPage>(Mud.ManPages.Where(p => p.Name == manPageName));
                        if (pages.Count > 0)
                            foreach (var manPage in pages)
                                manPage.SendManPage(actor);
                        else
                            Mud.SendMessage(actor, "No help for that topic.");

                    }
                    return PerformResult.Continue;
                });
               
		}
	}
}

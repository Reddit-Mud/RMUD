using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Help : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("HELP"),
                        KeyWord("?")),
                    Optional(SingleWord("COMMAND"))),
                "Display a list of all defined commands.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.Arguments.ContainsKey("COMMAND"))
                    {
                        Mud.SendMessage(actor, "Available help topics");
                        var line = "";
                        foreach (var manPage in Mud.ManPages)
                        {
                            line += manPage.Name;
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
                        var manPage = Mud.ManPages.FirstOrDefault(p => p.Name == manPageName);
                        if (manPage != null)
                            manPage.SendManPage(actor);
                        else
                            Mud.SendMessage(actor, "No help for that topic.");

                    }
                    return PerformResult.Continue;
                });
               
		}
	}
}

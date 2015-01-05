using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Meta
{
	internal class Man : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("HELP"),
                        KeyWord("MAN"),
                        KeyWord("?")),
                    Optional(Rest("TOPIC"))))
                .Manual("This is the command you typed to get this message.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("TOPIC"))
                    {
                        MudObject.SendMessage(actor, "Available help topics");
                        var line = "";
                        foreach (var manPage in ManPages.Pages.Select(p => p.Name).Distinct().OrderBy(s => s))
                        {
                            line += manPage;
                            if (line.Length < 20) line += new String(' ', 20 - line.Length);
                            else if (line.Length < 40) line += new String(' ', 40 - line.Length);
                            else
                            {
                                MudObject.SendMessage(actor, line);
                                line = "";
                            }
                        }
                    }
                    else
                    {
                        var manPageName = match["TOPIC"].ToString().ToUpper();
                        var pages = new List<ManPage>(ManPages.Pages.Where(p => p.Name == manPageName));
                        if (pages.Count > 0)
                            foreach (var manPage in pages)
                                manPage.SendManPage(actor);
                        else
                            MudObject.SendMessage(actor, "No help for that topic.");

                    }
                    return PerformResult.Continue;
                });
               
		}
	}
}

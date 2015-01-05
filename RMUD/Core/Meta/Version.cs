using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Meta
{
	internal class Version : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Or(
                    KeyWord("VERSION"),
                    KeyWord("VER")))
                .Manual("Displays the server version currently running.")
                .ProceduralRule((match, actor) =>
                {
                    var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    MudObject.SendMessage(actor, String.Format("Build: RMUD Hadad {0}", buildVersion));

                    if (System.IO.File.Exists("version.txt"))
                        MudObject.SendMessage(actor, String.Format("Commit: {0}", System.IO.File.ReadAllText("version.txt")));
                    else
                        MudObject.SendMessage(actor, "Commit version not found.");
                    return PerformResult.Continue;
                });
		}
	}
}

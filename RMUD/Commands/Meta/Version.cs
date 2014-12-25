using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Version : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Or(
                    KeyWord("VERSION"),
                    KeyWord("VER")),
                "See what version the server is running.")
                .Manual("Displays the server version currently running.")
                .ProceduralRule((match, actor) =>
                {
                    var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    Mud.SendMessage(actor, String.Format("Build: RMUD Hadad {0}", buildVersion));

                    if (System.IO.File.Exists("version.txt"))
                        Mud.SendMessage(actor, String.Format("Commit: {0}", System.IO.File.ReadAllText("version.txt")));
                    else
                        Mud.SendMessage(actor, "Commit version not found.");
                    return PerformResult.Continue;
                });
		}
	}
}

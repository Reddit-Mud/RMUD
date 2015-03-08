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
            Core.StandardMessage("version", "Build: RMUD Hadad <s0>");
            Core.StandardMessage("commit", "Commit: <s0>");
            Core.StandardMessage("no commit", "Commit version not found.");

            Parser.AddCommand(
                Or(
                    KeyWord("VERSION"),
                    KeyWord("VER")))
                .Manual("Displays the server version currently running.")
                .ProceduralRule((match, actor) =>
                {
                    var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    MudObject.SendMessage(actor, "@version", buildVersion);

                    if (System.IO.File.Exists("version.txt"))
                        MudObject.SendMessage(actor, "@commit", System.IO.File.ReadAllText("version.txt"));
                    else
                        MudObject.SendMessage(actor, "@no commit");

                    foreach (var module in Core.ModuleAssemblies)
                        MudObject.SendMessage(actor, module.Info.Description);

                    return SharpRuleEngine.PerformResult.Continue;
                });
		}
	}
}

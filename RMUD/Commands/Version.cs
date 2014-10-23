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
                new Or(
			        new KeyWord("VERSION", false),
                    new KeyWord("VER", false)),
				new VersionProcessor(),
				"See what version the server is running.");
		}
	}

	internal class VersionProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;

            var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            Mud.SendMessage(Actor, String.Format("Build: RMUD Moneta {0}", buildVersion));

            if (System.IO.File.Exists("version.txt"))
                Mud.SendMessage(Actor, String.Format("Commit: {0}", System.IO.File.ReadAllText("version.txt")));
            else
                Mud.SendMessage(Actor, "Commit version not found.");
		}
	}
}

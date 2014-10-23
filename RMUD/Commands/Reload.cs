using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Reload : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new RankGate(500),
					new KeyWord("RELOAD", false),
                    new FailIfNoMatches(
					    new Path("TARGET"),
                        "It helps if you give me a path.")),
				new ReloadProcessor(),
				"Reload an object from disc.");

            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("RESET", false),
                    new FailIfNoMatches(
                        new Path("TARGET"),
                        "It helps if you give me a path.")),
                new ResetProcessor(),
                "Reset an object. It is not reloaded from disc.");
		}
	}

	internal class ReloadProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"].ToString();
			var newObject = Mud.ReloadObject(target, s =>
				{
					if (Actor.ConnectedClient != null)
						Mud.SendMessage(Actor, s);
				});

			if (Actor.ConnectedClient == null) return;

			if (newObject == null)
				Mud.SendMessage(Actor, "Failed to reload " + target);
			else
				Mud.SendMessage(Actor, "Reloaded " + target);				
		}
	}

    internal class ResetProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["TARGET"].ToString();
            var succeeded = Mud.ResetObject(target, s =>
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, s);
            });

            if (Actor.ConnectedClient == null) return;

            if (!succeeded)
                Mud.SendMessage(Actor, "Failed to reset " + target);
            else
                Mud.SendMessage(Actor, "Reset " + target);
        }
    }
}

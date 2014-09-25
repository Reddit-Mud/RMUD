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
					new Path("TARGET"))
				, new ReloadProcessor(),
				"Reload an object from disc.");

            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("RESET", false),
                    new Path("TARGET"))
                , new ResetProcessor(),
                "Reset an object. It is not reloaded from disc.");
		}
	}

	internal class ReloadProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"].ToString();
			var newObject = Mud.ReloadObject(target, s =>
				{
					if (Actor.ConnectedClient != null)
						Actor.ConnectedClient.Send(s + "\r\n");
				});

			if (Actor.ConnectedClient == null) return;

			if (newObject == null)
				Actor.ConnectedClient.Send("Failed to reload " + target + "\r\n");
			else
				Actor.ConnectedClient.Send("Reloaded " + target + "\r\n");				
		}
	}

    internal class ResetProcessor : ICommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["TARGET"].ToString();
            var succeeded = Mud.ResetObject(target, s =>
            {
                if (Actor.ConnectedClient != null)
                    Actor.ConnectedClient.Send(s + "\r\n");
            });

            if (Actor.ConnectedClient == null) return;

            if (!succeeded)
                Actor.ConnectedClient.Send("Failed to reset " + target + "\r\n");
            else
                Actor.ConnectedClient.Send("Reset " + target + "\r\n");
        }
    }
}

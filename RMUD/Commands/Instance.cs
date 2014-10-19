using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Instance : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("INSTANCE", false),
                    new FailIfNoMatches(
                        new Path("TARGET"),
                        "It helps if you give me a path.\r\n")),
                new InstanceProcessor(),
                "Create a new instance of an object.");
        }
	}

	internal class InstanceProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["TARGET"].ToString();
            var newObject = Mud.CreateInstance(target + "@" + Guid.NewGuid().ToString(), s =>
				{
					if (Actor.ConnectedClient != null)
						Mud.SendMessage(Actor, s + "\r\n");
				});

            if (newObject == null)
                Mud.SendMessage(Actor, "Failed to instance " + target + "\r\n");
            else
            {
                MudObject.Move(newObject, Actor);
                Mud.SendMessage(Actor, "Instanced " + target + "\r\n");
            }
		}
	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Go : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
                new FirstOf(
                    new Sequence(
                        new KeyWord("GO", false),
                        new FailIfNoMatches(
                            new Cardinal("DIRECTION"),
                            "What way was that?")),
                    new Cardinal("DIRECTION")),
				new GoProcessor(),
				"Move between rooms.");
		}
	}

	internal class GoProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var direction = Match.Arguments["DIRECTION"] as Direction?;
			var location = Actor.Location as Room;
            var link = location.EnumerateObjects().FirstOrDefault(thing => thing is Link && (thing as Link).Direction == direction.Value) as Link;

			if (link == null)
				Mud.SendMessage(Actor, "You can't go that way.");
			else
			{
                if (link.Portal != null)
                {
                    if (GlobalRules.ConsiderValueRule<bool>("openable?", link.Portal, link.Portal))
                    {
                        if (!GlobalRules.ConsiderValueRule<bool>("open?", link.Portal, link.Portal))
                        {
                            Mud.SendMessage(Actor, "The door is closed.");
                            return;
                        }
                    }
                }

				Mud.SendMessage(Actor, "You went " + direction.Value.ToString().ToLower() + ".");
				Mud.SendExternalMessage(Actor, Actor.Short + " went " + direction.Value.ToString().ToLower() + ".");
				var destination = Mud.GetObject(link.Destination, s =>
				{
					if (Actor.ConnectedClient != null)
						Mud.SendMessage(Actor, s);
				}) as Room;
				if (destination == null) throw new InvalidOperationException("[ERROR] Link does not lead to room.");
				MudObject.Move(Actor, destination);
				Mud.EnqueuClientCommand(Actor.ConnectedClient, "look");

                var arriveMessage = Link.FromMessage(Link.Opposite(direction.Value));

				Mud.SendExternalMessage(Actor, Actor.Short + " arrives " + arriveMessage + ".");

                Mud.MarkLocaleForUpdate(location);
                Mud.MarkLocaleForUpdate(destination);
			}
		}
	}
}

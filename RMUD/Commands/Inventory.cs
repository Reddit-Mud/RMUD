using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Inventory : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Or(
					new KeyWord("i", false),
					new KeyWord("inv", false),
					new KeyWord("inventory", false)),
				new InventoryProcessor(),
				"See what you are carrying.");
		}
	}

	internal class InventoryProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			if (Actor.ConnectedClient == null) return;

			var builder = new StringBuilder();

            var wornObjects = Mud.GetContents(Actor as Container, RelativeLocations.Worn);
            if (wornObjects.Count == 0) builder.Append("You are naked.\r\n");
            else
            {
                builder.Append("You are wearing..\r\n");
                foreach (var item in wornObjects)
                {
                    builder.Append("  ");
                    builder.Append(item.Indefinite);
                    builder.Append("\r\n");
                }
            }

            var heldObjects = Mud.GetContents(Actor as Container, RelativeLocations.Held);
			if (heldObjects.Count == 0) builder.Append("You have nothing.");
			else
			{
				builder.Append("You are carrying..\r\n");
				foreach (var item in heldObjects)
				{
					builder.Append("  ");
					builder.Append(item.Indefinite);
					builder.Append("\r\n");
				}
			}

			Mud.SendMessage(Actor, builder.ToString());
		}
	}
}

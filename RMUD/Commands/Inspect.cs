using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Inspect : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new RankGate(500),
					new Or(
						new KeyWord("INSPECT", false),
						new KeyWord("INS", false),
						new KeyWord("P", false)),
                    new FailIfNoMatches(
					    new Or(
						    new ObjectMatcher("OBJECT", new InScopeObjectSource()),
						    new KeyWord("HERE", false)),
                        "I don't see that here.\r\n")),
				new InspectProcessor(),
				"Inspect internal properties of an object.");
		}
	}

	internal class InspectProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			if (Actor.ConnectedClient == null) return;

			Object target = null;
			if (Match.Arguments.ContainsKey("OBJECT")) target = Match.Arguments["OBJECT"];
			else target = Actor.Location;

			var data = new StringBuilder();
			data.Append(target.GetType().Name);
			data.Append("\r\n");

			foreach (var @interface in target.GetType().GetInterfaces())
			{
				data.Append("Implements ");
				data.Append(@interface.Name);
				data.Append("\r\n");
			}

			foreach (var field in target.GetType().GetFields())
			{
				data.Append(field.FieldType.Name);
				data.Append(" ");
				data.Append(field.Name);
				data.Append(" = ");
				var value = field.GetValue(target);
				if (value == null) data.Append("null");
				else data.Append(value.ToString());
				data.Append("\r\n");
			}

			foreach (var property in target.GetType().GetProperties())
			{
				if (!property.CanWrite) data.Append("readonly ");
				data.Append(property.PropertyType.Name);
				data.Append(" ");
				data.Append(property.Name);
				if (property.CanRead)
				{
					data.Append(" = ");
					try
					{
						var value = property.GetValue(target, null);
						if (value == null) data.Append("null");
						else data.Append(value.ToString());
					}
					catch (Exception)
					{
						data.Append("[Error retrieving value]");
					}
				}

				data.Append("\r\n");
			}

			Mud.SendMessage(Actor, data.ToString());
		}
	}
}

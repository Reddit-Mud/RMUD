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
					new KeyWord("INSPECT", false),
					new FailIfNoMatches(
					    new Or(
						    new ObjectMatcher("OBJECT", new InScopeObjectSource()),
						    new KeyWord("HERE", false)),
                        "I don't see that here.")),
				new InspectProcessor(),
				"Inspect internal properties of an object.");
		}
	}

	internal class InspectProcessor : CommandProcessor
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
                WriteValue(data, field.GetValue(target));
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
                        WriteValue(data, property.GetValue(target, null));
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

        private static void WriteValue(StringBuilder To, Object Value)
        {
            if (Value == null)
                To.Append("NULL");
            else if (Value is String)
                To.Append("\"" + Value + "\"");
            else if (Value is MudObject)
                To.Append(Value.ToString());
            else if (Value is System.Collections.IEnumerable)
            {
                To.Append("[ ");
                int count = 0;
                foreach (var subValue in (Value as System.Collections.IEnumerable))
                {
                    count += 1;
                    WriteValue(To, subValue);
                    To.Append(", ");
                }

                if (count > 0) To.Remove(To.Length - 2, 2);
                To.Append(" ]");
            }
            else
                To.Append(Value.ToString());

        }
	}
}

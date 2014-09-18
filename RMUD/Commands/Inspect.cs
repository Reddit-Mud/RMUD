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
					new Or(
						new ObjectMatcher("TARGET"),
						new KeyWord("HERE", false)))
				, new InspectProcessor(),
				"Inspect internal properties of an object.");
		}
	}

	internal class InspectProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			if (Actor.ConnectedClient == null) return;

			Object target = null;
			if (Match.Arguments.ContainsKey("TARGET")) target = Match.Arguments["TARGET"];
			else target = Actor.Location;

			var data = new StringBuilder();
			data.Append(target.GetType().Name);
			data.Append("\r\n");
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

			Actor.ConnectedClient.Send(data.ToString());
		}
	}
}

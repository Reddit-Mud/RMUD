using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Who : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
			    new KeyWord("WHO", false),
				new WhoProcessor(),
				"See who is logged on.");
		}
	}

	internal class WhoProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;

            var clients = Mud.ConnectedClients.Where(c => c.IsLoggedOn);

            var builder = new StringBuilder();
            builder.Append("~~~ WHO IS ONLINE ~~~\r\n");

            foreach (var client in clients)
            {
                builder.Append(client.Player.Short);
                builder.Append(" [");
                builder.Append(client.ConnectionDescription);
                builder.Append("]");
                if (client.Player.Location != null)
                {
                    builder.Append(" -- ");
                    builder.Append(client.Player.Location.Path);
                }
                builder.Append("\r\n");                
            }

            builder.Append("\r\n");

            Actor.ConnectedClient.Send(builder.ToString());
		}
	}
}

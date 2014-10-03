using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD
{
	public class LoginCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

			Parser.AddCommand(
				new Sequence(new KeyWord("LOGIN", false), new SingleWord("NAME")),
				new CommandProcessorWrapper((m, a) =>
				{
                    var client = m.Arguments["CLIENT"] as Client;
                    client.Player = new Actor();
					client.Player.Short = m.Arguments["NAME"].ToString();
                    client.Player.Nouns.Add(client.Player.Short.ToUpper());
                    client.Player.ConnectedClient = client;
					client.CommandHandler = Mud.ParserCommandHandler;
					client.Rank = 500; //Everyone is a wizard! 
                    Mud.FindChatChannel("OOC").Subscribers.Add(client); //Everyone is on ooc!
					MudObject.Move(client.Player,
                        Mud.GetObject(
                            (Mud.GetObject("settings") as Settings).NewPlayerStartRoom, 
                            s => Mud.SendMessage(client, s + "\r\n")));
					Mud.EnqueuClientCommand(client, "look");
				}),
				"Login to an existing account.");
		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, null);
                if (matchedCommand != null)
                {
                    matchedCommand.Matches[0].Arguments.Upsert("CLIENT", Client);
                    matchedCommand.Command.Processor.Perform(matchedCommand.Matches[0], null);
                }
                else
                    Mud.SendMessage(Client, "I do not understand.");
			}
			catch (Exception e)
			{
				Mud.ClearPendingMessages();
                Mud.SendMessage(Client, e.Message);
			}
		}
	}
}

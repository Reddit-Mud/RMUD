using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class ReportError : CommandProcessor
	{
		public String Message;

		public ReportError(String Message)
		{
			this.Message = Message;
		}

		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor == null)
            {
                if (Match.Arguments.ContainsKey("CLIENT"))
                {
                    var client = Match.Arguments["CLIENT"] as Client;
                    if (client != null)
                        Mud.SendMessage(client, Message);
                }
            }
            else
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, Message);
            }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Ban : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
			        new KeyWord("BANS", false)),
				new ShowBansProcessor(),
				"Display the ban list.");

            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("BAN", false),
                    new Optional(new SingleWord("GLOB")),
                    new Optional(new Rest("REASON"))),
                new BanProcessor(),
                "Ban an ip address.");

            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("UNBAN", false),
                    new Optional(new SingleWord("GLOB"))),
                new UnbanProcessor(),
                "Remove a ban on an ip address.");
		}
	}

	internal class ShowBansProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            if (Actor.ConnectedClient == null) return;

            var builder = new StringBuilder();
            builder.Append("~~~ ALL SET BANS ~~~\r\n");

            foreach (var proscription in Mud.ProscriptionList.Proscriptions)
            {
                builder.Append(proscription.Glob);
                builder.Append(" : ");
                builder.Append(proscription.Reason);
                builder.Append("\r\n");
            }
            
            Mud.SendMessage(Actor, builder.ToString());
		}
	}

    internal class BanProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (!Match.Arguments.ContainsKey("GLOB"))
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "You need to supply a wildcard mask for the ip address.");
                return;
            }

            if (!Match.Arguments.ContainsKey("REASON"))
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "You must state a reason for the ban.");
                return;
            }

            var glob = Match.Arguments["GLOB"].ToString();

            var reasonBuilder = new StringBuilder();
            Mud.AssembleText(Match.Arguments["REASON"] as LinkedListNode<String>, reasonBuilder);
            var reason = reasonBuilder.ToString();

            Mud.ProscriptionList.Ban(glob, reason);

            if (Actor.ConnectedClient != null)
                Mud.SendMessage(Actor, "You banned " + glob);
        }
    }

    internal class UnbanProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (!Match.Arguments.ContainsKey("GLOB"))
            {
                if (Actor.ConnectedClient != null) Mud.SendMessage(Actor, "You need to supply the wildcard mask to unban.");
                return;
            }

            var glob = Match.Arguments["GLOB"].ToString();
            
            Mud.ProscriptionList.RemoveBan(glob);

            if (Actor.ConnectedClient != null)
                Mud.SendMessage(Actor, "You unbanned " + glob);
        }
    }
}

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
                "Display the ban list.")
                .ProceduralRule((match, actor) =>
                {
                    Mud.SendMessage(actor, "~~~ ALL SET BANS ~~~");
                    foreach (var proscription in Mud.ProscriptionList.Proscriptions)
                        Mud.SendMessage(actor, proscription.Glob + " : " + proscription.Reason);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("BAN"),
                    MustMatch("You must supply an ip mask.", SingleWord("GLOB")),
                    MustMatch("You must supply a reason.", Rest("REASON"))),
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

	internal class BanProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
           

            var glob = Match.Arguments["GLOB"].ToString();
            var reason = Match.Arguments["REASON"].ToString();

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

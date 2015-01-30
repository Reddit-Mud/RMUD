using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
{
    internal class Ban : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                KeyWord("BANS"))
                .Manual("Lists all active bans.")
                .ProceduralRule((match, actor) =>
                {
                    MudObject.SendMessage(actor, "~~~ ALL SET BANS ~~~");
                    foreach (var proscription in Clients.ProscriptionList.Proscriptions)
                        MudObject.SendMessage(actor, proscription.Glob + " : " + proscription.Reason);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("BAN"),
                    MustMatch("You must supply an ip mask.", SingleWord("GLOB")),
                    MustMatch("You must supply a reason.", Rest("REASON"))))
                .Manual("Ban every player who's ip matches the mask.")
                .ProceduralRule((match, actor) =>
                {
                    Clients.ProscriptionList.Ban(match["GLOB"].ToString(), match["REASON"].ToString());
                    Clients.SendGlobalMessage("^<the0> has banned " + match["GLOB"].ToString(), actor);
                    return PerformResult.Continue;
                });

            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("UNBAN"),
                    MustMatch("You must supply an ip mask.", SingleWord("GLOB"))))
                .Manual("Remove an existing ban.")
                .ProceduralRule((match, actor) =>
                {
                    Clients.ProscriptionList.RemoveBan(match["GLOB"].ToString());
                    Clients.SendGlobalMessage("^<the0> removes the ban on " + match["GLOB"].ToString(), actor);
                    return PerformResult.Continue;
                });
        }
    }
}
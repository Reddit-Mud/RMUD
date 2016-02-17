using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace QuestModule
{
    internal class AcceptQuest : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("ACCEPT"),
                    KeyWord("QUEST")))
                .Name("ACCEPT QUEST")
                .Manual("When an NPC offers you a quest, you will be prompted to accept the quest. Accepting a quest abandons any quest you already have active.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.GetProperty<MudObject>("offered-quest") == null)
                    {
                        MudObject.SendMessage(actor, "Nobody has offered you a quest.");
                        return SharpRuleEngine.PerformResult.Stop;
                    }
                    else
                    {
                        match.Upsert("QUEST", actor.GetProperty<MudObject>("offered-quest"));
                        return SharpRuleEngine.PerformResult.Continue;
                    }
                }, "the must have been offered a quest, and bookeeping rule.")
                .ProceduralRule((match, actor) =>
                {
                    if (!Core.GlobalRules.ConsiderValueRule<bool>("quest available?", actor, actor.GetProperty<MudObject>("offered-quest")))
                    {
                        MudObject.SendMessage(actor, "The quest is no longer available.");
                        actor.SetProperty("offered-quest", null);
                        return SharpRuleEngine.PerformResult.Stop;
                    }
                    return SharpRuleEngine.PerformResult.Continue;
                }, "the quest must be available rule.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor.GetProperty<MudObject>("active-quest") != null)
                        Core.GlobalRules.ConsiderPerformRule("quest abandoned", actor, actor.GetProperty<MudObject>("active-quest"));

                    actor.SetProperty("active-quest", actor.GetProperty<MudObject>("offered-quest"));
                    actor.SetProperty("offered-quest", null);
                    return SharpRuleEngine.PerformResult.Continue;
                }, "the any active quest must be abandoned rule.")
                .Perform("quest accepted", "ACTOR", "QUEST");
        }
    }
}
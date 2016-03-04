using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using RMUD;

namespace QuestModule
{
    public class QuestProceduralRules 
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("active-quest", typeof(MudObject), null, new DefaultSerializer());
            PropertyManifest.RegisterProperty("offered-quest", typeof(MudObject), null, new DefaultSerializer());

            GlobalRules.Perform<PossibleMatch, MudObject>("after acting")
                .Do((match, actor) =>
                {
                    if (actor.GetProperty<MudObject>("active-quest") != null)
                    {
                        var quest = actor.GetProperty<MudObject>("active-quest");
                                                
                        if (GlobalRules.ConsiderValueRule<bool>("quest complete?", actor, quest))
                        {
                            actor.SetProperty("active-quest", null);
                            GlobalRules.ConsiderPerformRule("quest completed", actor, quest);
                        }
                        else if (GlobalRules.ConsiderValueRule<bool>("quest failed?", actor, quest))
                        {
                            actor.SetProperty("active-quest", null);
                            GlobalRules.ConsiderPerformRule("quest failed", actor, quest);
                        }
                    }

                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Check quest status after acting rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest reset", "[quest, thing] : The quest is being reset. Quests can call this on objects they interact with.", "quest", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest accepted", "[actor, quest] : Handle accepting a quest.", "actor", "quest");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest completed", "[actor, quest] : Handle when a quest is completed.", "actor", "quest");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest failed", "[actor, quest] : Handle when a quest is failed.", "actor", "quest");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest abandoned", "[actor, quest] : Handle when a quest is abandoned.", "actor", "quest");

            GlobalRules.Perform<MudObject, MudObject>("quest abandoned")
                .Last
                .Do((actor, quest) =>
                {
                    return GlobalRules.ConsiderPerformRule("quest failed", actor, quest);
                })
                .Name("Abandoning a quest is failure rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest available?", "[actor, quest -> bool] : Is the quest available to this actor?", "actor", "quest");

            GlobalRules.Value<MudObject, MudObject, bool>("quest available?")
                .Do((Actor, quest) => false)
                .Name("Quests unavailable by default rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest complete?", "[actor, quest -> bool] : Has this actor completed this quest?", "actor", "quest");

            GlobalRules.Value<MudObject, MudObject, bool>("quest complete?")
                .Do((actor, quest) => false)
                .Name("Quests incomplete by default rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest failed?", "[actor, quest -> bool] : Has this actor failed this quest?", "actor", "quest");

            GlobalRules.Value<MudObject, MudObject, bool>("quest failed?")
                .Do((actor, quest) => false)
                .Name("Quests can't fail by default rule.");

        }
    }
}

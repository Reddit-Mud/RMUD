using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Space
{
	internal class Hit : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                BestScore("OBJEcT",
                    Sequence(
                        Or(
                            KeyWord("HIT"),
                            KeyWord("ATTACK"),
                            KeyWord("SMASH"),
                            KeyWord("BREAK")),
                        MustMatch("@not here",
                            Object("SUBJECT", InScope)),
                        OptionalKeyWord("WITH"),
                        MustMatch("@not here",
                            Object("OBJECT", InScope, PreferHeld)))))
                .Manual("Hit an object with another object.")
                .Check("can hit with?", "ACTOR", "SUBJECT", "OBJECT")
                .BeforeActing()
                .Perform("hit with", "ACTOR", "SUBJECT", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can hit with?", "[Actor, Subject, Object] : Can the SUBJECT be hit by the ACTOR with the OBJECT?");
            
            GlobalRules.Check<MudObject, MudObject, MudObject>("can hit with?")
                .Do((actor, subject, @object) => MudObject.CheckIsVisibleTo(actor, subject))
                .Name("Item must be visible to hit it.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can hit with?")
                .Do((actor, subject, @object) => MudObject.CheckIsHolding(actor, @object))
                .Name("Object must be held rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can hit with?")
                .Do((a, b, c) => CheckResult.Allow)
                .Name("Default allow hitting rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("hit with", "[Actor, Subject, Object] : Handle the actor hitting the subject with the object.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("hit with").Do((actor, subject, @object) =>
            {
                MudObject.SendMessage(actor, "I smasked <the0> with <the1>. I don't think it did anything.", subject, @object);
                return PerformResult.Stop;
            });
        }
    }
}

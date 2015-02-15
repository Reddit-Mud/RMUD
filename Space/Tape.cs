using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace Space
{
	internal class Tape : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                BestScore("SUBJECT",
                    Sequence(
                        KeyWord("TAPE"),
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, PreferHeld)),
                        OptionalKeyWord("TO"),
                        MustMatch("@not here",
                            Object("OBJECT", InScope)))))
                .Manual("Tape one thing to another")
                .Check("can tape to?", "ACTOR", "SUBJECT", "OBJECT")
                .BeforeActing()
                .Perform("taped to", "ACTOR", "SUBJECT", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject>("can tape to?", "[Actor, Subject, Object] : Can the subject be taped to the object?");
            
            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .Do((actor, subject, @object) => MudObject.CheckIsVisibleTo(actor, @object))
                .Name("Object must be visible to tape something to it rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .Do((actor, subject, @object) => MudObject.CheckIsHolding(actor, subject))
                .Name("Subject must be held to tape it to something rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .When((actor, subject, @object) => !MudObject.ObjectContainsObject(actor, MudObject.GetObject("DuctTape")))
                .Do((actor, subject, @object) =>
                {
                    MudObject.SendMessage(actor, "You don't have any tape.");
                    return CheckResult.Disallow;
                })
                .Name("Need tape to tape rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .When((actor, subject, @object) => subject.GetPropertyOrDefault<Weight>("weight", Weight.Normal) != Weight.Light)
                .Do((actor, subject, @object) =>
                {
                    MudObject.SendMessage(actor, "^<the0> is too heavy to tape to things.", subject);
                    return CheckResult.Disallow;
                })
                .Name("Can only tape light things rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
               .When((actor, subject, @object) => !(@object is Container) || ((@object as Container).Supported & RelativeLocations.On) != RelativeLocations.On)
               .Do((actor, subject, @object) =>
               {
                   MudObject.SendMessage(actor, "I can't tape things to <the0>.", @object);
                   return CheckResult.Disallow;
               })
               .Name("Can only tape things to containers that support 'on' rule");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .Do((a, b, c) => CheckResult.Allow)
                .Name("Default allow taping things to things rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("taped to", "[Actor, Subject, Object] : Handle the actor taping the subject to the object.");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("taped to").Do((actor, subject, @object) =>
            {
                MudObject.SendMessage(actor, "Okay, I taped <the0> onto <the1>.", subject, @object);
                return PerformResult.Continue;
            });
        }
    }
}

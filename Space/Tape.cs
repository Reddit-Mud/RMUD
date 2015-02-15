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
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "ACTOR", "SUBJECT", "KEY")
                .BeforeActing()
                .Perform("locked", "ACTOR", "SUBJECT", "KEY")
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
                .When((actor, subject, @object) => MudObject.ObjectContainsObject(actor, MudObject.GetObject("DuctTape")))
                .Do((actor, subject, @object) =>
                {
                    MudObject.SendMessage(actor, "You don't have any tape.");
                    return CheckResult.Disallow;
                })
                .Name("Need tape to tape rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject>("can tape to?")
                .Do((a, b, c) => CheckResult.Allow)
                .Name("Default allow taping things to things rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject>("locked", "[Actor, Item, Key] : Handle the actor locking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<MudObject, MudObject, MudObject>("locked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "@you lock", target);
                MudObject.SendExternalMessage(actor, "@they lock", actor, target, key);
                return PerformResult.Continue;
            });
        }
    }
}

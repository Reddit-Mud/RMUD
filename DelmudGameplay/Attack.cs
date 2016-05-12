using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace DelmudGameplay
{
    public class Attack : RMUD.CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
               Sequence(
                   Or(
                        KeyWord("ATTACK"),
                        KeyWord("KILL")),
                   Optional(
                        Object("OBJECT", InScope, (actor, item) =>
                        {
                            if (item.GetProperty<bool>("actor?")) return MatchPreference.Likely;
                            else return MatchPreference.Unlikely;
                        }))))
                .ID("Delmud:Attack")
                .Manual("Attack an enemy.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("OBJECT"))
                        match.Upsert("OBJECT", actor.GetProperty<MudObject>("combatant"));
                    return SharpRuleEngine.PerformResult.Continue;
                }, "Attack current combantant by default rule.")
                .Check("can attack?", "ACTOR", "OBJECT")
                .ProceduralRule((match, actor) =>
                {
                    var combatant = match["OBJECT"] as MudObject;

                    if (combatant == null)
                    {
                        MudObject.SendMessage(actor, "@combat nobody");
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    actor.SetProperty("combatant", combatant);
                    return SharpRuleEngine.PerformResult.Continue;

                }, "Set actor combatant rule.")
                .BeforeActing()
                .Perform("attack", "ACTOR", "OBJECT")
                .AfterActing()
                .MarkLocaleForUpdate();

        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            RMUD.PropertyManifest.RegisterProperty("combatant", typeof(MudObject), null, new DefaultSerializer());

            RMUD.Core.StandardMessage("combat nobody", "I can't find who you want to attack.");
            RMUD.Core.StandardMessage("combat cant attack", "You can't attack that.");
            Core.StandardMessage("combat show damage", "<a0> hits <a1> for <s2> damage!");
            Core.StandardMessage("combat show damage self", "You hit <a1> for <s2> damage!");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can attack?", "[Aggresor, Victim] : Can the aggresor attack the victim?");

            GlobalRules.Check<MudObject, MudObject>("can attack?")
                .When((actor, item) => !item.GetProperty<bool>("actor?"))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@combat cant attack");
                    return SharpRuleEngine.CheckResult.Disallow;
                })
                .Name("Can only attack actors rule");

            GlobalRules.Check<MudObject, MudObject>("can attack?")
                .Do((actor, victim) => MudObject.CheckIsVisibleTo(actor, victim))
                .Name("Victim must be visible rule.");

            GlobalRules.Check<MudObject, MudObject>("can attack?")
                .Last
                .Do((actor, victim) => SharpRuleEngine.CheckResult.Allow)
                .Name("Let the violence commence rule.");


            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("attack", "[Aggresor, Victim] : Handle the aggresor attacking the victim.");

            GlobalRules.Perform<MudObject, MudObject>("attack")
                .Do((actor, victim) =>
                {
                    var attackPower = 0;

                    // Check for weapons.

                    // No weapons, unarmed strike.
                    attackPower = actor.GetProperty<int>("strength");

                    // Display how much damage was done.
                    MudObject.SendExternalMessage(actor, "@combat show damage", actor, victim, attackPower);
                    MudObject.SendMessage(actor, "@combat show damage self", actor, victim, attackPower);

                    // Apply attack power to health of victim - use 'damage' rulebook
                    //  No need to account for defense here - let the damage taking rulebook handle absorbant armor.
                    GlobalRules.ConsiderPerformRule("damage", actor, victim, attackPower);

                    return SharpRuleEngine.PerformResult.Continue;
                });


            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, int>("damage", "[Attacker, Target, Damage] : Handle the Attacker dealing Damage damage to the Target.");

            GlobalRules.Perform<MudObject, MudObject, int>("damage")
                .Do((attacker, target, damage) =>
                {
                    var hp = target.GetProperty<int>("current-hp");
                    hp = Math.Max(0, hp - damage);
                    target.SetProperty("current-hp", hp);

                    if (hp == 0)
                        GlobalRules.ConsiderPerformRule("killed", target, attacker);

                    return SharpRuleEngine.PerformResult.Continue;
                })
                .Name("Apply damage to actor rule.");
        }
    }
}

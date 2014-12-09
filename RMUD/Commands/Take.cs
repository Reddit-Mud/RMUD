using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Take : CommandFactory, DeclaresRules
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("GET", false),
						new KeyWord("TAKE", false)),
                    new FailIfNoMatches(
					    new ObjectMatcher("SUBJECT", new InScopeObjectSource(), 
                            (actor, MudObject) => {
                                if (actor.Contains(MudObject, RelativeLocations.Held)) return MatchPreference.VeryUnlikely;
                                //Prefer MudObjects that can actually be taken
                                if (GlobalRules.ConsiderActionRuleSilently("can-take", MudObject, actor, MudObject) != RuleResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            }),
                        "I don't see that here.")),
                new TakeProcessor(),
				"Take something",
                "SUBJECT-SCORE");
		}

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("can-take", "[Actor, Thing], Action rule to determine if a thing can be taken.");
            GlobalRules.DeclareActionRuleBook<MudObject, MudObject>("on-taken", "[Actor, Thing], Action rule to handle taken event.");

            GlobalRules.AddActionRule<MudObject, MudObject>("can-take").Do((a, t) => RuleResult.Allow);

            GlobalRules.AddActionRule<MudObject, MudObject>("on-taken").Do((actor, target) =>
                {
                    Mud.SendMessage(actor, "You take <a0>.", target);
                    Mud.SendExternalMessage(actor, "<a0> takes <a1>.", actor, target);
                    MudObject.Move(target, actor);
                    return RuleResult.Continue;
                });
        }
    }

	internal class TakeProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments.ValueOrDefault("SUBJECT") as MudObject;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            if (Actor.Contains(target, RelativeLocations.Held))
            {
                Mud.SendMessage(Actor, "You are already holding that.");
                return;
            }

            if (GlobalRules.ConsiderActionRule("can-take", target, Actor, target) == RuleResult.Allow)
                GlobalRules.ConsiderActionRule("on-taken", target, Actor, target);

            Mud.MarkLocaleForUpdate(target);
        }
	}
}

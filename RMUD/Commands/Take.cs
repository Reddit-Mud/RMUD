using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Take : CommandFactory
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
                                if (MudObject is TakeRules && !(MudObject as TakeRules).Check(actor).Allowed)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            }),
                        "I don't see that here.")),
                new TakeProcessor(),
				"Take something",
                "SUBJECT-SCORE");
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

            var takeRules = target as TakeRules;
            if (takeRules != null)
            {
                var checkRule = takeRules.Check(Actor);
                if (!checkRule.Allowed)
                {
                    Mud.SendMessage(Actor, checkRule.ReasonDisallowed);
                    return;
                }
            }

            var handleRuleFollowUp = RuleHandlerFollowUp.Continue;
            if (takeRules != null) handleRuleFollowUp = takeRules.Handle(Actor);

            if (handleRuleFollowUp == RuleHandlerFollowUp.Continue)
            {
                Mud.SendMessage(Actor, "You take " + target.Indefinite + ".");
                Mud.SendExternalMessage(Actor, Actor.Short + " takes " + target.Indefinite + ".");
                MudObject.Move(target, Actor);
            }

            Mud.MarkLocaleForUpdate(target);
        }
	}
}

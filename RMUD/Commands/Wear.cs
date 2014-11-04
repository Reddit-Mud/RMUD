using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Wear : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("WEAR", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), ObjectMatcher.PreferHeld),
                        "I couldn't figure out what you're trying to wear.")),
                new WearProcessor(),
                "Wear something.",
                "OBJECT-SCORE");
        }
	}
	
	internal class WearProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["OBJECT"] as WearableRules;
            var MudObject = target as MudObject;

			if (target == null)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "I don't see how you would manage that.");
				return;
			}

			if (!Mud.ObjectContainsObject(Actor, MudObject))
			{
				if (Actor.ConnectedClient != null)
					Mud.SendMessage(Actor, "You'd have to be holding " + MudObject.Definite + " for that to work.");
				return;
			}

            if (Actor.LocationOf(target as MudObject) == RelativeLocations.Worn)
            {
                Mud.SendMessage(Actor, "You're already wearing that.");
                return;
            }

            var checkRule = target.CheckWear(Actor);
            if (checkRule.Allowed)
            {
                if (target.HandleWear(Actor) == RuleHandlerFollowUp.Continue)
                {
                        Mud.SendMessage(Actor, "You wear " + MudObject.Definite + ".");
                        Mud.SendExternalMessage(Actor, Actor.Short + " wears " + MudObject.Indefinite + ".");
                    Mud.Move(MudObject, Actor, RelativeLocations.Worn);
                }
            }
            else
            {
                Mud.SendMessage(Actor, checkRule.ReasonDisallowed);
            }
		}
	}

}

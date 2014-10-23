using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Open : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("OPEN", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("SUBJECT", new InScopeObjectSource(),
                             (actor, openable) =>
                             {
                                 if (openable is OpenableRules && !(openable as OpenableRules).Open)
                                     return MatchPreference.Likely;
                                 return MatchPreference.Unlikely;
                             }),
                        "I don't see that here.")),
                new OpenProcessor(),
                "Open something",
                "SUBJECT-SCORE");
        }
	}
	
	internal class OpenProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
			var target = Match.Arguments["SUBJECT"] as OpenableRules;
			if (target == null)
			{
				if (Actor.ConnectedClient != null) 
					Mud.SendMessage(Actor, "I don't think the concept of 'open' and 'closed' applies to that.");
			}
			else
			{
                if (!Mud.IsVisibleTo(Actor, target as MudObject))
            {
                if (Actor.ConnectedClient != null)
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

                var checkRule = target.CheckOpen(Actor);
				if (checkRule.Allowed)
				{
                    if (target.HandleOpen(Actor) == RuleHandlerFollowUp.Continue)
                    {
                        target.Open = true;

                        var MudObject = target as MudObject;
                        if (MudObject != null)
                        {
                            Mud.SendMessage(Actor, "You open " + MudObject.Definite + ".");
                            Mud.SendExternalMessage(Actor, Actor.Short + " opens " + MudObject.Definite + ".");
                        }
                    }

                    Mud.MarkLocaleForUpdate(target as MudObject);
				}
				else
				{
					Mud.SendMessage(Actor, checkRule.ReasonDisallowed);
				}
			}
		}
	}
}

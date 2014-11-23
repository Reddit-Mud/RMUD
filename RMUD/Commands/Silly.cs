using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
    internal class Silly : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                 new Sequence(
                    new KeyWord("SILLY"),
                     new FailIfNoMatches(
                         new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                             {
                                 if (Object is RMUD.Actor) return MatchPreference.Likely;
                                 else return MatchPreference.Unlikely;
                             }),
                         "Silly whom?")),
                 new SillyProcessor(),
                 "SILLY SILLY SILLY");
        }
    }

    internal class SillyProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["OBJECT"] as MudObject;
            var introductee = target as Actor;

            if (introductee == null)
            {
                Mud.SendMessage(Actor, "That just sounds silly.");
                return;
            }

            if (!Mud.IsVisibleTo(Actor, introductee))
            {
                Mud.SendMessage(Actor, "^<the0> does not seem to be here anymore you silly goose.", introductee);
                return;
            }

            Mud.SendExternalMessage(Actor, "^<the0> applies extra silly to <the1>.", Actor, introductee);
            Mud.SendMessage(Actor, "You apply extra silly to <the0>.", introductee);
            introductee.ApplyStatusEffect(new SillyStatusEffect());
        }
    }
}

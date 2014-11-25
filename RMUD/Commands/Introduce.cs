using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Introduce : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("INTRODUCE"),
                    new Or(
                        new KeyWord("MYSELF"),
                        new KeyWord("ME"),
                        new KeyWord("SELF"))),
                new IntroduceSelfProcessor(),
                "Introduce yourself.");

            Parser.AddCommand(
                new Sequence(
                   new KeyWord("INTRODUCE"),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource(), (Actor, Object) =>
                            {
                                if (Object is RMUD.Actor) return MatchPreference.Likely;
                                else return MatchPreference.Unlikely;
                            }),
                        "Introduce whom?")),
                new IntroduceProcessor(),
                "Introduce someone.");
        }
	}

	internal class IntroduceProcessor : CommandProcessor
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
                Mud.SendMessage(Actor, "^<the0> does not seem to be here anymore.", introductee);
                return;
            }

            if (!Introduction.ActorKnowsActor(Actor, introductee))
            {
                Mud.SendMessage(Actor, "How can you introduce <the0> when you don't know them yourself?", introductee);
                return;
            }

            Introduction.Introduce(introductee);
            Mud.SendExternalMessage(Actor, "^<the0> introduces the " + introductee.DescriptiveName + " as <the1>.", Actor, introductee);
            Mud.SendMessage(Actor, "You introduce <the0>.", introductee);
		}
	}

    internal class IntroduceSelfProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            Introduction.Introduce(Actor);
            Mud.SendExternalMessage(Actor, "The " + Actor.DescriptiveName + " introduces themselves as <the0>.", Actor);
            Mud.SendMessage(Actor, "You introduce yourself.");
        }
    }
}

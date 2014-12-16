using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{

    internal class Lycanthropy : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("TRANSFORM")),
                new TransformProcessor(),
                "Transform into a wolf.");
        }
    }

    internal class TransformProcessor : CommandProcessor
    {
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            //if (Actor.HasStatusEffect(typeof(LycanthropeStatusEffect)))
            //{
            //    Mud.SendMessage(Actor, "You transform into a human.");
            //    Mud.SendExternalMessage(Actor, "<the0> transforms into a human.", Actor);
            //    Actor.RemoveStatusEffect(Actor.GetStatusEffect<LycanthropeStatusEffect>());
            //}
            //else
            //{
            //    Mud.SendMessage(Actor, "You transform into a wolf.");
            //    Mud.SendExternalMessage(Actor, "<the0> transforms into a wolf.", Actor);
            //    Actor.ApplyStatusEffect(new LycanthropeStatusEffect());
            //}
        }
    }
}

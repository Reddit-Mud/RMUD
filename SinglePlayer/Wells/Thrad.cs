using RMUD;
using System;
using RMUD.Modules.Conversation;

namespace Wells
{
    public class Thrad : RMUD.NPC
    {
        public override void Initialize()
        {
            Nouns.Add("THRAD", a => GlobalRules.ConsiderValueRule<bool>("actor knows actor?", a, this));
            Nouns.Add("KNIGHT");

            Short = "Thrad";

            Perform<MudObject, Thrad>("describe in locale")
                .When((actor, thrad) => !GlobalRules.ConsiderValueRule<bool>("actor knows actor?", actor, thrad))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "A massive knight stands in the middle of the little room.");
                    return PerformResult.Continue;
                });

            Value<Actor, Thrad, String, String>("printed name")
                .When((viewer, thrad, article) => !GlobalRules.ConsiderValueRule<bool>("actor knows actor?", viewer, thrad))
                .Do((viewer, actor, article) => article + " knight");

            this.Response("who he is", (actor, npc, topic) =>
                {
                    SendMessage(actor, "<the0> peers at you from within his incredible helmet. \"Thrad\", he says.", this);
                    GlobalRules.ConsiderPerformRule("introduce self", this);
                    return PerformResult.Stop;
                });

            Wear("helmet", ClothingLayer.Outer, ClothingBodyPart.Head);
            Wear("pauldrons", ClothingLayer.Outer, ClothingBodyPart.Cloak);

        }
    }   
}
using RMUD;
using System;

namespace Wells
{
    public class Thrad : RMUD.NPC
    {
        public override void Initialize()
        {
            Nouns.Add("THRAD", a => ActorKnowsActor(a, this));
            Nouns.Add("KNIGHT");

            Short = "Thrad";

            Perform<MudObject, MudObject>("describe in locale")
                .When((actor, item) => item == this && !ActorKnowsActor(actor as Actor, item as Actor))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "A massive knight stands in the middle of the little room.");
                    return PerformResult.Continue;
                });

            Value<MudObject, MudObject, String, String>("printed name")
                .When((viewer, thing, article) => thing == this && !ActorKnowsActor(viewer as Actor, thing as Actor))
                .Do((viewer, actor, article) => article + " knight");

            Response("who he is", (actor, npc, topic) =>
                {
                    SendMessage(actor, "<the0> peers at you from within his incredible helmet. \"Thrad\", he says.", this);
                    Introduce(this);
                    return PerformResult.Stop;
                });

            Wear("helmet", ClothingLayer.Outer, ClothingBodyPart.Head);
            Wear("pauldrons", ClothingLayer.Outer, ClothingBodyPart.Cloak);

        }
    }   
}
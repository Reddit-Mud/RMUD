using RMUD;
using System;
using ConversationModule;
using ClothingModule;

namespace Wells
{
    public class Thrad : RMUD.NPC
    {
        //This does, essentially, what the entire IntroductionModule does. This is quite tedius to setup for every NPC, but is straightforward nonetheless.
        public bool Introduced = false;

        public override void Initialize()
        {
            Nouns.Add("THRAD", a => this.Introduced);
            Nouns.Add("KNIGHT");

            Short = "Thrad";

            Perform<MudObject, Thrad>("describe in locale")
                .When((actor, thrad) => !this.Introduced)
                .Do((actor, item) =>
                {
                    SendMessage(actor, "A massive knight stands in the middle of the little room.");
                    return PerformResult.Continue;
                });

            Value<Actor, Thrad, String, String>("printed name")
                .When((viewer, thrad, article) => !this.Introduced)
                .Do((viewer, actor, article) => article + " knight");

            this.Response("who he is", (actor, npc, topic) =>
                {
                    SendMessage(actor, "<the0> peers at you from within his incredible helmet. \"Thrad\", he says.", this);
                    this.Introduced = true;
                    return PerformResult.Stop;
                });

            this.Wear("helmet", ClothingLayer.Outer, ClothingBodyPart.Head);
            this.Wear("pauldrons", ClothingLayer.Outer, ClothingBodyPart.Cloak);

        }
    }   
}
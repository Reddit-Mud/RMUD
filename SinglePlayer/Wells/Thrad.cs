using RMUD;
using System;
using ConversationModule;
using ClothingModule;

namespace Wells
{
    public class Thrad : RMUD.NPC
    {
        //This does, essentially, what the entire IntroductionModule does. 
        //This is quite tedius to setup for every NPC, but is straightforward for just one.
        public bool Introduced = false;

        public override void Initialize()
        {
            Nouns.Add("THRAD", a => this.Introduced);
            Nouns.Add("KNIGHT", "MASSIVE");

            Short = "Thrad";

            Perform<MudObject, Thrad>("describe in locale") //Draw Thrad to the player's attention if they 
                .When((actor, thrad) => !this.Introduced) //haven't spoken to him yet.
                .Do((actor, item) =>
                {
                    SendMessage(actor, "A massive knight stands in the middle of the little room.");
                    return SharpRuleEngine.PerformResult.Continue;
                });


            this.Wear("helmet", ClothingLayer.Outer, ClothingBodyPart.Head);
            this.Wear("pauldrons", ClothingLayer.Outer, ClothingBodyPart.Cloak);

            //We want Thrad to be 'a knight' or 'Thrad' depending on if he is introduced.
            Value<Actor, Thrad, String, String>("printed name")
                .When((viewer, thrad, article) => !this.Introduced)
                .Do((viewer, actor, article) => article + " knight");

            Value<Actor, Thrad, String, String>("printed name")
                .When((viewer, thrad, article) => this.Introduced)
                .Do((viewer, actor, article) => "Thrad");


            //The conversation with Thrad
            var who_he_is = this.Response("who he is", (actor, npc, topic) =>
                {
                    SendMessage(actor, "^<the0> peers at you from within his incredible helmet. \"Thrad\", he says.", this);
                    this.Introduced = true;
                    return SharpRuleEngine.PerformResult.Stop;
                });

            var helmet = this.Response("about his incredible helmet", "\"This?\" Thrad rumbles from inside the helmet. \"This is nothing. This is to keep my brain from splattering on walls. Instead it splatters on inside of helmet. Much easier to cleanup.\"").Available(() => who_he_is.Discussed);
        }
    }   
}
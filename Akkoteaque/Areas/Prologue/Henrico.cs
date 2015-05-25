using RMUD;
using ConversationModule;

namespace Akko.Areas.Prologue
{
    public class Henrico : RMUD.NPC
    {
        public override void Initialize()
        {
            Short = "Mr. Henrico";
            Nouns.Add("MR", "MR.", "HENRICO");

            Perform<RMUD.MudObject, RMUD.MudObject>("describe in locale").Do((actor, item) =>
            {
                SendMessage(actor, "Mr. Henrico is driving the car.");
                return SharpRuleEngine.PerformResult.Continue;
            });

            Long = "Mr. Henrico is a balding, middle aged man with a moustache and a squinty face. The combination thereof makes it seem like he's captured a squirrel under his nose and is squeezing it as tightly as possible to avoid it escaping. He's been rather kind to you, or at least, kinder than you expected after the series of social workers you had to deal with before him.";

            this.DefaultResponse("\"Hmm,\" Mr. Henrico says, and then nothing else.");

            var conversationCounter = 0;

            Perform<MudObject, MudObject, MudObject>("topic response")
                .When((actor, npc, topic) => topic != null)
                .Do((actor, npc, topic) =>
            {
                conversationCounter += 1;
                return SharpRuleEngine.PerformResult.Continue;
            });

            #region Primary Conversation
            var a = this.Response("where we are going", "\"It's a very nice island,\" Mr. Henrico says. \"Agrotiki. Actor-tea? It's something like that.\"");

            var b = this.Response("why you have to live on an island", "\"Well you don't, I suppose. The island isn't the important qualification,\" Mr. Henrico explains. \"Your grandmother lives on the island, so you have to live on the island. It's very cut and dry. Only having one living relative makes it fairly simple to place you.\"");

            var c = this.Response("what he knows about your grandmother", "\"I wouldn't worry too much about those awful things your mother told you about her.\"").Follows(b);
            #endregion

            #region Secondary Conversation Topics

            this.Response("about your watch", "\"Are you still carrying that around?\" Mr Henrico asks. He drives in silence for a moment. \"I've spoken to you so many times about closure.\"").Available(() => GetObject("Areas.Prologue.Watch").GetBooleanProperty("has-been-viewed"), "Watch response is available after watch examined rule.");

            #endregion

            //var d = this.Response("what he knows about the island"

        }
    }   
}
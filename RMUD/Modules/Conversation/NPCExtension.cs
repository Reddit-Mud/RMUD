using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Topic : MudObject
    {
        public Topic Available(Func<MudObject, MudObject, MudObject, bool> Func)
        {
            Value<MudObject, MudObject, MudObject, bool>("topic available").Do(Func);
            return this;
        }
    }

    public partial class NPC : Actor
    {
        public List<MudObject> ConversationTopics = new List<MudObject>();

        public Topic Response(String Topic, String StringResponse)
        {
            return Response(Topic, (actor, npc, topic) =>
                {
                    MudObject.SendMessage(actor, StringResponse, npc);
                    return PerformResult.Stop;
                });
        }

        public Topic Response(String Topic, Func<MudObject, MudObject, MudObject, PerformResult> FuncResponse)
        {
            var response = new Topic();
            ConversationTopics.Add(response);
            response.SimpleName(Topic);
            response.Perform<MudObject, MudObject, MudObject>("topic response").Do(FuncResponse);
            return response;
        }
    }
}

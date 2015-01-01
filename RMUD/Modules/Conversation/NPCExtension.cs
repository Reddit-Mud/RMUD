using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class NPC : Actor
    {
        public List<MudObject> ConversationTopics = new List<MudObject>();

        public MudObject Response(String Topic, String StringResponse)
        {
            return Response(Topic, (actor, npc, topic) =>
                {
                    MudObject.SendMessage(actor, StringResponse, npc);
                    return PerformResult.Stop;
                });
        }

        public MudObject Response(String Topic, Func<MudObject, MudObject, MudObject, PerformResult> FuncResponse)
        {
            var response = new MudObject();
            ConversationTopics.Add(response);
            response.SimpleName(Topic);
            response.Perform<MudObject, MudObject, MudObject>("topic response").Do(FuncResponse);
            return response;
        }
    }
}

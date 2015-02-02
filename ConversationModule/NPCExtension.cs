using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ConversationModule
{
    public class Topic : MudObject
    {
        public Topic()
        {
            Article = "";
        }

        public Topic Available(Func<MudObject, MudObject, MudObject, bool> Func)
        {
            Value<MudObject, MudObject, MudObject, bool>("topic available?").Do(Func);
            return this;
        }
    }

    public static class ResponseExtensionMethods
    {
        public static Topic Response(this MudObject To, String Topic, String StringResponse)
        {
            return Response(To, Topic, (actor, npc, topic) =>
                {
                    MudObject.SendMessage(actor, StringResponse, npc);
                    return PerformResult.Stop;
                });
        }

        public static Topic Response(this MudObject To, String Topic, Func<MudObject, MudObject, MudObject, PerformResult> FuncResponse)
        {
            var topics = To.GetProperty<List<MudObject>>("conversation-topics");
            if (topics == null)
            {
                topics = new List<MudObject>();
                To.SetProperty("conversation-topics", topics);
            }

            var response = new Topic();
            topics.Add(response);
            response.SimpleName(Topic);
            response.Perform<MudObject, MudObject, MudObject>("topic response").Do(FuncResponse);
            return response;
        }
    }
}

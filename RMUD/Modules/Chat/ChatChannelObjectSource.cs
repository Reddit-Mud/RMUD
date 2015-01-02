using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Chat
{
    public class ChatChannelObjectSource : IObjectSource
    {
        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            return new List<MudObject>(ChatChannel.ChatChannels);
        }
    }
}

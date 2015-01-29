using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ChatModule
{
    public class ChatChannelObjectSource : IObjectSource
    {
        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            return new List<MudObject>(ChatChannel.ChatChannels);
        }
    }
}

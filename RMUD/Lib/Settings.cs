using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Settings : MudObject
	{
		public String Banner;
		public String MessageOfTheDay;
        public String NewPlayerStartRoom;
        public bool UpfrontCompilation = false;

        public int AllowedCommandRate = 100; //How many milliseconds to allow between commands - default is to not limit very much.

        public String ProscriptionList = "proscriptions.txt";
        public int MaximumChatChannelLogSize = 1000;

        public Dictionary<int, String> RankNames;

        public Settings()
        {
            RankNames = new Dictionary<int, string>();
            RankNames.Add(Int32.MaxValue, "deum confugiat");
            RankNames.Add(500, "deus");
            RankNames.Add(400, "imperator");
            RankNames.Add(300, "patrician");
            RankNames.Add(200, "senator");
            RankNames.Add(100, "equester");
            RankNames.Add(0, "proletarian");
            RankNames.Add(Int32.MinValue, "sentina");

            Mud.ChatChannels.Add(new ChatChannel("OOC"));
            Mud.ChatChannels.Add(new ChatChannel("SENATE", c => c.Rank >= 100));
            Mud.ChatChannels.Add(new ChatChannel("HELP"));
        }

        public String GetNameForRank(int Rank)
        {
            foreach (var entry in RankNames)
            {
                if (entry.Key <= Rank) return entry.Value;
            }

            return "errorem magnificum";
        }
	}
}

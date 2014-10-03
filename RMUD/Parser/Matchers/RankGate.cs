using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class RankGate : CommandTokenMatcher
    {
        public int RequiredRank;

		internal RankGate(int RequiredRank)
		{
			this.RequiredRank = RequiredRank;
		}

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
            if (Context.ExecutingActor.ConnectedClient != null && Context.ExecutingActor.ConnectedClient.Rank >= RequiredRank)
                R.Add(State);
            else
                throw new CommandParser.MatchAborted("You do not have sufficient rank to use that command.\r\n");
			return R;
        }

		public String Emit() { return "<Rank must be >= " + RequiredRank + ">"; }
    }
}

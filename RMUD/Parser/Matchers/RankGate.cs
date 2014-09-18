using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class RankGate : ICommandTokenMatcher
    {
        public int RequiredRank;

		internal RankGate(int RequiredRank)
		{
			this.RequiredRank = RequiredRank;
		}

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			if (Context.ExecutingActor.Rank >= RequiredRank)
				R.Add(State);
			return R;
        }

		public String Emit() { return "<Rank must be >= " + RequiredRank + ">"; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class FilterGate : ICommandTokenMatcher
    {
        public Func<PossibleMatch, CommandParser.MatchContext, bool> Filter;

		internal FilterGate(Func<PossibleMatch, CommandParser.MatchContext, bool> Filter)
		{
			this.Filter = Filter;
		}

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			if (Filter(State, Context))
				R.Add(State);
			return R;
        }
    }
}

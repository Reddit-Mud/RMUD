using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Rest : ICommandTokenMatcher
    {
        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			R.Add(new PossibleMatch(State.Arguments, null));
			return R;
        }
    }
}

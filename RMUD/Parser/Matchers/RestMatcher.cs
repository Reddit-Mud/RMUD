using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Rest : ICommandTokenMatcher
    {
		public String ArgumentName;

		public Rest(String ArgumentName)
		{
			this.ArgumentName = ArgumentName;
		}

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			var possibleMatch = new PossibleMatch(State.Arguments, null);
			possibleMatch.Arguments.Upsert(ArgumentName, State.Next);
			R.Add(possibleMatch);
			return R;
        }
    }
}

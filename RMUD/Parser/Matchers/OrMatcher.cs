using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class Or : ICommandTokenMatcher
    {
        internal List<ICommandTokenMatcher> Matchers = new List<ICommandTokenMatcher>();

		public Or(params ICommandTokenMatcher[] matchers)
		{
			Matchers.AddRange(matchers);
		}

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var Matches = new List<PossibleMatch>();
            foreach (var Matcher in Matchers)
				Matches.AddRange(Matcher.Match(State, Context));
            return Matches;
        }

		public String Emit() { return "( " + String.Join(" | ", Matchers.Select(m => m.Emit())) + " )"; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class FirstOf : ICommandTokenMatcher
    {
        internal List<ICommandTokenMatcher> Matchers = new List<ICommandTokenMatcher>();

		public FirstOf(params ICommandTokenMatcher[] matchers)
		{
			Matchers.AddRange(matchers);
		}

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var Matches = new List<PossibleMatch>();
            foreach (var Matcher in Matchers)
            {
                Matches.AddRange(Matcher.Match(State, Context));
                if (Matches.Count > 0) return Matches;
            }
            return Matches;
        }

		public String Emit() { return "( " + String.Join(" | ", Matchers.Select(m => m.Emit())) + " )"; }
    }

}

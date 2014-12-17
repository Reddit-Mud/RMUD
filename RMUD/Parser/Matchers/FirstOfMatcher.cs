using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher FirstOf(params CommandTokenMatcher[] Subs)
        {
            return new FirstOf(Subs);
        }
    }

    public class FirstOf : CommandTokenMatcher
    {
        internal List<CommandTokenMatcher> Matchers = new List<CommandTokenMatcher>();

		public FirstOf(params CommandTokenMatcher[] matchers)
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

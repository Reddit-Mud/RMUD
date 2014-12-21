using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Sequence(params CommandTokenMatcher[] matchers)
        {
            return new Sequence(matchers);
        }
    }

    public class Sequence : CommandTokenMatcher
    {
        internal List<CommandTokenMatcher> Matchers = new List<CommandTokenMatcher>();

        public Sequence(params CommandTokenMatcher[] matchers)
        {
            Matchers.AddRange(matchers);
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var Matches = new List<PossibleMatch>();
            Matches.Add(State);
            foreach (var Matcher in Matchers)
            {
                var NextMatches = new List<PossibleMatch>();
                foreach (var Match in Matches)
                    NextMatches.AddRange(Matcher.Match(Match, Context));
                Matches = NextMatches;
                if (Matches.Count == 0) return Matches; //Shortcircuit for when no matches are found.
            }
            return Matches;
        }

        public String FindFirstKeyWord()
        {
            foreach (var sub in Matchers)
            {
                var s = sub.FindFirstKeyWord();
                if (!String.IsNullOrEmpty(s)) return s;
            }
            return null;
        }

        public String Emit() { return String.Join(" ", Matchers.Select(m => m.Emit())); }
    }
}

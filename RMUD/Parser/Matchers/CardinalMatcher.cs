using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Cardinal : ICommandTokenMatcher
    {
        public String ArgumentName;

        internal Cardinal(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var r = new List<PossibleMatch>();
			if (Link.IsCardinal(State.Next.Value.ToUpper()))
			{
				var match = new PossibleMatch(State.Arguments, State.Next.Next);
				match.Arguments.Upsert(ArgumentName, Link.ToCardinal(State.Next.Value.ToUpper()));
				r.Add(match);
			}
			return r;
        }

		public String Emit() { return "[CARDINAL]"; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Path : CommandTokenMatcher
    {
        public String ArgumentName;

        internal Path(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			if (State.Next != null)
			{
				var match = new PossibleMatch(State.Arguments, State.Next.Next);
				match.Arguments.Upsert(ArgumentName, State.Next.Value);
				R.Add(match);
			}
			return R;
        }

		public String Emit() { return "[PATH]"; }
    }
}

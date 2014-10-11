using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Rest : CommandTokenMatcher
    {
		public String ArgumentName;

		public Rest(String ArgumentName)
		{
			this.ArgumentName = ArgumentName;
		}

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
            if (State.Next != null)
                r.Add(State.EndWith(ArgumentName, State.Next));
			return r;
        }

		public String Emit() { return "[TEXT]"; }
    }
}

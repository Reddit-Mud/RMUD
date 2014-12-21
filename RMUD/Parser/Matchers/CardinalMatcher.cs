using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Cardinal(String ArgumentName)
        {
            return new Cardinal(ArgumentName);
        }
    }

    internal class Cardinal : CommandTokenMatcher
    {
        public String ArgumentName;

        internal Cardinal(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
			if (State.Next == null) return r;

            if (Link.IsCardinal(State.Next.Value.ToUpper()))
                r.Add(State.AdvanceWith(ArgumentName, Link.ToCardinal(State.Next.Value.ToUpper())));

			return r;
        }

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "[CARDINAL]"; }
    }
}

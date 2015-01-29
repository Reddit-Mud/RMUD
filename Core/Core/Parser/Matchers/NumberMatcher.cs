using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Number(String CaptureName)
        {
            return new Number(CaptureName);
        }
    }

    internal class Number : CommandTokenMatcher
    {
        public String ArgumentName;

        internal Number(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
			if (State.Next == null) return r;

            int value = 0;
            if (Int32.TryParse(State.Next.Value, out value))
                r.Add(State.AdvanceWith(ArgumentName, value));

			return r;
        }

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "[NUMBER]"; }
    }
}

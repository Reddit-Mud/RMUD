using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher RelativeLocation(String CaptureName)
        {
            return new RelativeLocationMatcher(CaptureName);
        }
    }

    internal class RelativeLocationMatcher : CommandTokenMatcher
    {
        public String ArgumentName;

        internal RelativeLocationMatcher(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();
			if (State.Next == null) return r;

            var word = State.Next.Value.ToUpper();
            if (word == "ON")
                r.Add(State.AdvanceWith(ArgumentName, RelativeLocations.On));
            else  if (word == "IN")
                r.Add(State.AdvanceWith(ArgumentName, RelativeLocations.In));
            else if (word == "UNDER")
                r.Add(State.AdvanceWith(ArgumentName, RelativeLocations.Under));
            else if (word == "BEHIND")
                r.Add(State.AdvanceWith(ArgumentName, RelativeLocations.Behind));
			return r;
        }

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "[RELLOC]"; }
    }
}

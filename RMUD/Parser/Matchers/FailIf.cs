using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class FailIf : CommandTokenMatcher
    {
        public Func<PossibleMatch, MatchContext, bool> Test;
        public String Message;

        public FailIf(Func<PossibleMatch, MatchContext, bool> Test, String Message)
        {
            this.Test = Test;
            this.Message = Message;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            if (Test(State, Context)) throw new CommandParser.MatchAborted(Message);
            var r = new List<PossibleMatch>();
            r.Add(State);
            return r;
        }

        public String Emit() { return "[FAIL-IF]"; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class FailIfNoMatches : ICommandTokenMatcher
    {
        public ICommandTokenMatcher Sub;
        public String Message;

        public FailIfNoMatches(ICommandTokenMatcher Sub, String Message)
        {
            this.Sub = Sub;
            this.Message = Message;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
            R.AddRange(Sub.Match(State, Context));
            if (R.Count == 0) throw new CommandParser.MatchAborted(Message);
            return R;
        }

        public String Emit() { return Sub.Emit(); }
    }
}

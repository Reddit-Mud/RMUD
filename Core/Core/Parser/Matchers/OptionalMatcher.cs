using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Optional(CommandTokenMatcher Sub, String BooleanProperty = null)
        {
            return new Optional(Sub, BooleanProperty);
        }
    }

    internal class Optional : CommandTokenMatcher
    {
        public CommandTokenMatcher Sub;
        public String BooleanProperty;

        public Optional(CommandTokenMatcher Sub, String BooleanProperty = null)
        {
            this.Sub = Sub;
            this.BooleanProperty = BooleanProperty;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
            if (String.IsNullOrEmpty(BooleanProperty))
            {
                R.AddRange(Sub.Match(State, Context));
                if (R.Count == 0) R.Add(State);
            }
            else
            {
                R.AddRange(Sub.Match(State, Context).Select(s => s.With(BooleanProperty, true)));
                if (R.Count == 0) R.Add(State.With(BooleanProperty, false));
            }
            return R;
        }

        public String FindFirstKeyWord() { return Sub.FindFirstKeyWord(); }
        public String Emit() { return Sub.Emit() + "?"; }
    }
}

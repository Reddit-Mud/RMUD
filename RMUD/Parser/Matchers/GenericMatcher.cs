using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Generic(Func<PossibleMatch, MatchContext, List<PossibleMatch>> MatchFunc, String HelpDescription)
        {
            return new GenericMatcher(MatchFunc, HelpDescription);
        }
    }

    internal class GenericMatcher : CommandTokenMatcher
    {
        public Func<PossibleMatch, MatchContext, List<PossibleMatch>> MatchFunc;
        public String HelpDescription;

        public GenericMatcher(Func<PossibleMatch, MatchContext, List<PossibleMatch>> MatchFunc, String HelpDescription)
        {
            this.MatchFunc = MatchFunc;
            this.HelpDescription = HelpDescription;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            return MatchFunc(State, Context);
        }

        public String FindFirstKeyWord() { return null; }
        public String Emit() { return HelpDescription; }
    }
}

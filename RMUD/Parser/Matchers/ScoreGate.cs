using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class ScoreGate : CommandTokenMatcher
    {
        public String ScoreArgument;
        public CommandTokenMatcher Sub;

        internal ScoreGate(CommandTokenMatcher Sub, String ScoreArgument)
        {
            this.ScoreArgument = ScoreArgument;
            this.Sub = Sub;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = Sub.Match(State, Context);
            var highestScoreFound = MatchPreference.VeryUnlikely;
            foreach (var match in r)
            {
                var score = GetScore(match, ScoreArgument);
                if (score > highestScoreFound) highestScoreFound = score;
            }
            return new List<PossibleMatch>(r.Where(m => highestScoreFound == GetScore(m, ScoreArgument)));
        }

        public String Emit() { return "<BEST " + Sub.Emit() + ">"; }

        private static MatchPreference GetScore(PossibleMatch Match, String ScoreArgumentName)
        {
            if (Match.Arguments.ContainsKey(ScoreArgumentName))
            {
                var argScore = Match.Arguments[ScoreArgumentName] as MatchPreference?;
                if (argScore.HasValue) return argScore.Value;
            }

            return MatchPreference.Plausible; //If there is no score, the match is neutral.
        }
    }
}

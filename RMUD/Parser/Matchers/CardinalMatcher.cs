using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class Cardinal : ICommandTokenMatcher
    {
        public String ArgumentName;

        internal Cardinal(String ArgumentName)
        {
			this.ArgumentName = ArgumentName;
        }

        public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
        {
            var r = new List<PossibleMatch>();
			var parsedDirection = Direction.NORTH;
			if (Enum.TryParse<Direction>(State.Next.Value.ToUpper(), out parsedDirection))
			{
				var match = new PossibleMatch(State.Arguments, State.Next.Next);
				match.Arguments.Upsert(ArgumentName, parsedDirection);
				r.Add(match);
			}
			return r;
        }
    }
}

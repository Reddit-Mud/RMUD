using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface ICommandTokenMatcher
    {
        List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context);
		String Emit();
    }
}

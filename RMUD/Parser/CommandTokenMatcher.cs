using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface CommandTokenMatcher
    {
        List<PossibleMatch> Match(PossibleMatch State, MatchContext Context);
		String Emit();
    }
}

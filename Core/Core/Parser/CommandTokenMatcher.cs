using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface CommandTokenMatcher
    {
        /// <summary>
        /// Find all possible matches. Matches returned should be ordered best to worst.
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Context"></param>
        /// <returns>An empty list if no match, otherwise a list of possible matches</returns>
        List<PossibleMatch> Match(PossibleMatch State, MatchContext Context);

        /// <summary>
        /// Find the first KeyWord matcher in the matcher tree, or null, if no KeyWord matcher can be found.
        /// </summary>
        /// <returns>Null, or the word matched by the first KeyWord matcher in the matcher tree.</returns>
        String FindFirstKeyWord();

        /// <summary>
        /// Format this matcher in a friendly string.
        /// </summary>
        /// <returns>The string description of this matcher</returns>
		String Emit();
    }
}

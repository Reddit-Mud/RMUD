using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher KeyWord(String Word)
        {
            return new KeyWord(Word);
        }

        public static CommandTokenMatcher OptionalKeyWord(String Word)
        {
            return new KeyWord(Word, true);
        }
    }

    public class KeyWord : CommandTokenMatcher
    {
        public String Word;
        public bool Optional = false;

        internal KeyWord(String Word, bool Optional = false)
        {
            this.Word = Word.ToUpper();
            this.Optional = Optional;
        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();
			if (State.Next != null && State.Next.Value.ToUpper() == Word)
                R.Add(State.Advance());
            else if (Optional) //Greedy match
                R.Add(State);
            return R;
        }

        public String FindFirstKeyWord() { return Word; }
		public String Emit() { return Optional ? (Word.ToLower() + "?") : Word.ToLower(); }
    }
}

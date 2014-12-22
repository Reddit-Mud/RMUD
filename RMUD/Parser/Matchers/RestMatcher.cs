using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher Rest(String ArgumentName)
        {
            return new Rest(ArgumentName);
        }
    }

    internal class Rest : CommandTokenMatcher
    {
		public String ArgumentName;

		public Rest(String ArgumentName)
		{
			this.ArgumentName = ArgumentName;
		}

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var r = new List<PossibleMatch>();

            if (State.Next != null)
            {
                var builder = new StringBuilder();
                var node = State.Next;
                for (; node != null; node = node.Next)
                {
                    builder.Append(node.Value);
                    builder.Append(" ");
                }

                builder.Remove(builder.Length - 1, 1);
                r.Add(State.EndWith(ArgumentName, builder.ToString()));
            }

			return r;
        }

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "[TEXT => '" + ArgumentName + "']"; }
    }
}

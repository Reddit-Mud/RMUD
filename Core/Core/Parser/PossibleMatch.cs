using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class PossibleMatch : Dictionary<String, Object>
    {
        public LinkedListNode<String> Next = null;

        public PossibleMatch(LinkedListNode<String> Next)
        {
            this.Next = Next;
        }

        private PossibleMatch(Dictionary<String, Object> Arguments, LinkedListNode<String> Next)
        {
            this.Next = Next;
            foreach (var pair in Arguments)
                this.Upsert(pair.Key, pair.Value);
        }

        public PossibleMatch Clone()
        {
            return new PossibleMatch(this, Next);
        }

        /// <summary>
        /// Clone this match, but give the clone an additional argument.
        /// </summary>
        /// <param name="ArgumentName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PossibleMatch With(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, Next);
            r.Upsert(ArgumentName, Value);
            return r;
        }

        /// <summary>
        /// Clone this match, but give the clone many additional arguments.
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        public PossibleMatch With(Dictionary<String, Object> Arguments)
        {
            var r = Clone();
            foreach (var arg in Arguments) r.Upsert(arg.Key, arg.Value);
            return r;
        }

        /// <summary>
        /// Clone this match, but advance the clone to the next token.
        /// </summary>
        /// <returns></returns>
        public PossibleMatch Advance()
        {
            return new PossibleMatch(this, Next.Next);
        }

        /// <summary>
        /// Clone this match, but advance the clone to the next token and give it an additional property.
        /// </summary>
        /// <param name="ArgumentName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PossibleMatch AdvanceWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, Next.Next);
            r.Upsert(ArgumentName, Value);
            return r;
        }

        /// <summary>
        /// Clone this match, but advance the clone to the end of input and give it an additional property.
        /// </summary>
        /// <param name="ArgumentName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PossibleMatch EndWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, null);
            r.Upsert(ArgumentName, Value);
            return r;
        }
    }
}

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

        public PossibleMatch With(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, Next);
            r.Upsert(ArgumentName, Value);
            return r;
        }

        public PossibleMatch Advance()
        {
            return new PossibleMatch(this, Next.Next);
        }

        public PossibleMatch AdvanceWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, Next.Next);
            r.Upsert(ArgumentName, Value);
            return r;
        }

        public PossibleMatch EndWith(String ArgumentName, Object Value)
        {
            var r = new PossibleMatch(this, null);
            r.Upsert(ArgumentName, Value);
            return r;
        }
    }
}

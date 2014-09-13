using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class PossibleMatch
    {
        public LinkedListNode<String> Next = null;
        public Dictionary<String, Object> Arguments = null;

        public PossibleMatch(LinkedListNode<String> Next)
        {
            this.Next = Next;
            Arguments = new Dictionary<String, Object>();
        }

		public PossibleMatch(Dictionary<String, Object> Arguments, LinkedListNode<String> Next)
		{
			this.Next = Next;
			this.Arguments = new Dictionary<String, Object>(Arguments);
		}
    }
}

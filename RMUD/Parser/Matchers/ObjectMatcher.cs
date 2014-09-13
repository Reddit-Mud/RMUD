using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    internal class ObjectMatcher : ICommandTokenMatcher
    {
		public String CaptureName;

		public ObjectMatcher(String CaptureName)
		{
			this.CaptureName = CaptureName;
		}

		public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
		{
			var R = new List<PossibleMatch>();
			foreach (var thing in Context.ObjectsInScope)
			{
				var possibleMatch = new PossibleMatch(State.Arguments, State.Next);
				while (possibleMatch.Next != null && thing.Adjectives.Contains(possibleMatch.Next.Value))
					possibleMatch.Next = possibleMatch.Next.Next;
				if (possibleMatch.Next == null) continue;
				if (thing.Nouns.Contains(possibleMatch.Next.Value))
				{
					possibleMatch.Next = possibleMatch.Next.Next;
					possibleMatch.Arguments.Upsert(CaptureName, thing);
					R.Add(possibleMatch);
				}
			}
			return R;
		}
    }
}

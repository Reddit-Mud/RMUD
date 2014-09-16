using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	[Flags]
	public enum ObjectMatcherSettings
	{
		None = 0,
		UnderstandMe = 1
	}

    public class ObjectMatcher : ICommandTokenMatcher
    {
		public String CaptureName;
		public ObjectMatcherSettings Settings;

		public ObjectMatcher(String CaptureName, ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
		{
			this.CaptureName = CaptureName;
			this.Settings = Settings;
		}

		public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
		{
			var R = new List<PossibleMatch>();

			if ((Settings & ObjectMatcherSettings.UnderstandMe) == ObjectMatcherSettings.UnderstandMe)
			{
				if (State.Next.Value.ToUpper() == "ME")
				{
					var possibleMatch = new PossibleMatch(State.Arguments, State.Next.Next);
					possibleMatch.Arguments.Upsert(CaptureName, Context.ExecutingActor);
					R.Add(possibleMatch);
				}
			}

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

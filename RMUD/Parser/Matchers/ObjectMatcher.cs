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

	public interface IObjectSource
	{
		List<IMatchable> GetObjects(PossibleMatch State, CommandParser.MatchContext Context);
	}

    public class ObjectMatcher : ICommandTokenMatcher
    {
		public String CaptureName;
		public ObjectMatcherSettings Settings;
		public IObjectSource ObjectSource;
		public Func<IMatchable, int> ScoreResults = null;

		public ObjectMatcher(
			String CaptureName,
			IObjectSource ObjectSource, 
			Func<IMatchable,int> ScoreResults = null,
			ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
		{
			this.CaptureName = CaptureName;
			this.ObjectSource = ObjectSource;
			this.ScoreResults = ScoreResults;
			this.Settings = Settings;
		}

		public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
		{
			var R = new List<PossibleMatch>();
			if (State.Next == null) return R;

			if ((Settings & ObjectMatcherSettings.UnderstandMe) == ObjectMatcherSettings.UnderstandMe)
			{
				if (State.Next.Value.ToUpper() == "ME")
				{
					var possibleMatch = new PossibleMatch(State.Arguments, State.Next.Next);
					possibleMatch.Arguments.Upsert(CaptureName, Context.ExecutingActor);
					R.Add(possibleMatch);
				}
			}

			foreach (var thing in ObjectSource.GetObjects(State, Context))
			{
				var possibleMatch = new PossibleMatch(State.Arguments, State.Next);
				bool matched = false;
				while (possibleMatch.Next != null && thing.Nouns.Contains(possibleMatch.Next.Value.ToUpper()))
				{
					matched = true;
					possibleMatch.Next = possibleMatch.Next.Next;
				}

				if (matched)
				{
					possibleMatch.Arguments.Upsert(CaptureName, thing);

					if (ScoreResults != null)
					{
						var score = ScoreResults(thing);
						possibleMatch.Arguments.Upsert("SCORE", score);

						var insertIndex = 0;
						for (insertIndex = 0; insertIndex < R.Count; ++insertIndex)
						{
							if (score > (R[insertIndex].Arguments["SCORE"] as int?).Value) break;
						}

						R.Insert(insertIndex, possibleMatch);
					}
					else
					{
						R.Add(possibleMatch);
					}
				}
			}
			return R;
		}

		public String Emit() { return "[OBJECT]"; }
    }
}

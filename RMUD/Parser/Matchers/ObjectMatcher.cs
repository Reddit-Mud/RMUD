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
        public String ScoreName;

		public ObjectMatcherSettings Settings;
		public IObjectSource ObjectSource;
		public Func<Actor, IMatchable, int> ScoreResults = null;

		public static int PreferNotHeld(Actor Actor, IMatchable Object)
		{
			if (Actor.Contains(Object)) return -1;
			return 0;
		}

		public static int PreferHeld(Actor Actor, IMatchable Object)
		{
			if (Actor.Contains(Object)) return 1;
			return 0;
		}

		public ObjectMatcher(
			String CaptureName,
			IObjectSource ObjectSource, 
			ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
		{
			this.CaptureName = CaptureName;
			this.ObjectSource = ObjectSource;
			this.Settings = Settings;
		}

        public ObjectMatcher(
            String CaptureName,
            IObjectSource ObjectSource,
            Func<Actor, IMatchable, int> ScoreResults,
            String ScoreName,
            ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
        {
            this.CaptureName = CaptureName;
            this.ObjectSource = ObjectSource;
            this.ScoreResults = ScoreResults;
            this.ScoreName = ScoreName;
            this.Settings = Settings;
        }

		public List<PossibleMatch> Match(PossibleMatch State, CommandParser.MatchContext Context)
		{
            var useObjectScoring = ScoreResults != null;

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

					if (useObjectScoring)
					{
						var score = ScoreResults(Context.ExecutingActor, thing);
						possibleMatch.Arguments.Upsert(ScoreName, score);

						var insertIndex = 0;
						for (insertIndex = 0; insertIndex < R.Count; ++insertIndex)
						{
							if (score > (R[insertIndex].Arguments[ScoreName] as int?).Value) break;
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

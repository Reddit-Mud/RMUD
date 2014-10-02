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

    public struct MatchableObject
    {
        public IMatchable Matchable;
        public RelativeLocations Source;

        public MatchableObject(IMatchable Matchable, RelativeLocations Source)
        {
            this.Matchable = Matchable;
            this.Source = Source;
        }
    }

	public interface IObjectSource
	{
		List<MatchableObject> GetObjects(PossibleMatch State, MatchContext Context);
	}

    public enum MatchPreference
    {
        VeryUnlikely = -2,
        Unlikely = -1,
        Plausible = 0,
        Likely = 1,
        VeryLikely = 2
    }

    public class ObjectMatcher : ICommandTokenMatcher
    {
		public String CaptureName;

		public ObjectMatcherSettings Settings;
		public IObjectSource ObjectSource;
		public Func<Actor, MudObject, MatchPreference> ScoreResults = null;

		public static MatchPreference PreferNotHeld(Actor Actor, MudObject Object)
		{
			if (Actor.Contains(Object, RelativeLocations.Held)) return MatchPreference.Unlikely;
			return MatchPreference.Plausible;
		}

		public static MatchPreference PreferHeld(Actor Actor, MudObject Object)
		{
			if (Actor.Contains(Object, RelativeLocations.Held)) return MatchPreference.Likely;
			return MatchPreference.Plausible;
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
            Func<Actor, MudObject, MatchPreference> ScoreResults,
            ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
        {
            this.CaptureName = CaptureName;
            this.ObjectSource = ObjectSource;
            this.ScoreResults = ScoreResults;
            this.Settings = Settings;
        }

		public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
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
                    possibleMatch.Arguments.Upsert(CaptureName + "-SOURCE", "ME");
					R.Add(possibleMatch);
				}
			}

			foreach (var matchableThing in ObjectSource.GetObjects(State, Context))
			{
				var possibleMatch = new PossibleMatch(State.Arguments, State.Next);
				bool matched = false;
				while (possibleMatch.Next != null && matchableThing.Matchable.Nouns.Contains(possibleMatch.Next.Value.ToUpper()))
				{
					matched = true;
					possibleMatch.Next = possibleMatch.Next.Next;
				}

				if (matched)
				{
					possibleMatch.Arguments.Upsert(CaptureName, matchableThing.Matchable);
                    possibleMatch.Arguments.Upsert(CaptureName + "-SOURCE", matchableThing.Source);

					if (useObjectScoring)
					{
						var score = ScoreResults(Context.ExecutingActor, matchableThing.Matchable as MudObject);
						possibleMatch.Arguments.Upsert(CaptureName + "-SCORE", score);

						var insertIndex = 0;
						for (insertIndex = 0; insertIndex < R.Count; ++insertIndex)
						{
							if (score > (R[insertIndex].Arguments[CaptureName + "-SCORE"] as MatchPreference?).Value) break;
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

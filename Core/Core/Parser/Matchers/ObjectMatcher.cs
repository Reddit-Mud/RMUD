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
		List<MudObject> GetObjects(PossibleMatch State, MatchContext Context);
	}

    public enum MatchPreference
    {
        VeryUnlikely = -2,
        Unlikely = -1,
        Plausible = 0,
        Likely = 1,
        VeryLikely = 2
    }

    public partial class CommandFactory
    {
        public static CommandTokenMatcher Object(
            String CaptureName,
            IObjectSource Source,
            ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
        {
            return new ObjectMatcher(CaptureName, Source, Settings);
        }

        public static CommandTokenMatcher Object(
            String CaptureName, 
            IObjectSource Source,
            Func<MudObject, MudObject, MatchPreference> ScoreFunc, 
            ObjectMatcherSettings Settings = ObjectMatcherSettings.UnderstandMe)
        {
            return new ObjectMatcher(CaptureName, Source, ScoreFunc, Settings);
        }

        public static IObjectSource InScope { get { return new InScopeObjectSource(); } }
        public static Func<MudObject, MudObject, MatchPreference> PreferHeld { get { return ObjectMatcher.PreferHeld; } }
        public static Func<MudObject, MudObject, MatchPreference> PreferWorn { get { return ObjectMatcher.PreferWOrn; } }
    }

    public class ObjectMatcher : CommandTokenMatcher
    {
		public String CaptureName;

		public ObjectMatcherSettings Settings;
		public IObjectSource ObjectSource;
		public Func<MudObject, MudObject, MatchPreference> ScoreResults = null;

		public static MatchPreference PreferNotHeld(MudObject Actor, MudObject Object)
		{
			if (Actor.Contains(Object, RelativeLocations.Held)) return MatchPreference.Unlikely;
			return MatchPreference.Plausible;
		}

		public static MatchPreference PreferHeld(MudObject Actor, MudObject Object)
		{
			if (Actor.Contains(Object, RelativeLocations.Held)) return MatchPreference.Likely;
			return MatchPreference.Plausible;
		}

        public static MatchPreference PreferWOrn(MudObject Actor, MudObject Object)
        {
            if (Actor.Contains(Object, RelativeLocations.Worn)) return MatchPreference.Likely;
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
            Func<MudObject, MudObject, MatchPreference> ScoreResults,
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
                    var possibleMatch = State.Advance();
					possibleMatch.Upsert(CaptureName, Context.ExecutingActor);
                    possibleMatch.Upsert(CaptureName + "-SOURCE", "ME");
					R.Add(possibleMatch);
				}
			}

			foreach (var matchableMudObject in ObjectSource.GetObjects(State, Context))
			{
                PossibleMatch possibleMatch = State;
				bool matched = false;
				while (possibleMatch.Next != null && matchableMudObject.GetProperty<NounList>("nouns").Match(possibleMatch.Next.Value.ToUpper(), Context.ExecutingActor))
				{
                    if (matched == false) possibleMatch = State.Clone();
					matched = true;
					possibleMatch.Next = possibleMatch.Next.Next;
				}

				if (matched)
				{
					possibleMatch.Upsert(CaptureName, matchableMudObject);

					if (useObjectScoring)
					{
						var score = ScoreResults(Context.ExecutingActor, matchableMudObject);
						possibleMatch.Upsert(CaptureName + "-SCORE", score);

						var insertIndex = 0;
						for (insertIndex = 0; insertIndex < R.Count; ++insertIndex)
						{
							if (score > (R[insertIndex][CaptureName + "-SCORE"] as MatchPreference?).Value) break;
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

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "[OBJECT => '" + CaptureName + "']"; }
    }
}

using RMUD;

namespace Space
{
    public class Hatch : Container
    {
        public Hatch() : base(RelativeLocations.On, RelativeLocations.On)
        {
            Short = "hatch";
            Long = "It looks just like every other hatch.";

            this.Nouns.Add("HATCH");
            this.Nouns.Add("CLOSED", h => !Open);
            this.Nouns.Add("OPEN", h => Open);
            Open = false;

            Value<MudObject, bool>("openable?").Do(a => true);
            Value<MudObject, bool>("open?").Do(a => Open);

            Check<MudObject, MudObject>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (Open)
                    {
                        MudObject.SendMessage(a, "@already open");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                })
                .Name("Can open doors rule.");

            Check<MudObject, MudObject>("can close?")
                .Last
                .Do((a, b) =>
                {
                    if (!Open)
                    {
                        MudObject.SendMessage(a, "@already closed");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                });

            Perform<MudObject, MudObject>("opened").Do((a, b) =>
            {
                Open = true;
                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide is Hatch) (otherSide as Hatch).Open = true;
                return PerformResult.Continue;
            });

            Perform<MudObject, MudObject>("closed").Do((a, b) =>
            {
                Open = false;
                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide is Hatch) (otherSide as Hatch).Open = false;
                return PerformResult.Continue;
            });
        }

        public bool Open { get; set; }
    }
}
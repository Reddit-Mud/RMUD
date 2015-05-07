using RMUD;

namespace Space
{
    public class Hatch : Container
    {
        ControlPanel ControlPanel;

        public Hatch()
            : base(RelativeLocations.On, RelativeLocations.On)
        {
            Short = "hatch";
            Long = "It looks just like every other hatch.";

            this.Nouns.Add("HATCH");
            this.Nouns.Add("CLOSED", h => !GetBooleanProperty("open?"));
            this.Nouns.Add("OPEN", h => GetBooleanProperty("open?"));

            SetProperty("open?", false);
            SetProperty("openable?", true);

            Check<MudObject, Hatch>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (GetBooleanProperty("open?"))
                    {
                        MudObject.SendMessage(a, "@already open");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                })
                .Name("Can open doors rule.");

            Check<MudObject, Hatch>("can close?")
                .Last
                .Do((a, b) =>
                {
                    if (!GetBooleanProperty("open?"))
                    {
                        MudObject.SendMessage(a, "@already closed");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                });

            Perform<MudObject, Hatch>("opened").Do((a, b) =>
            {
                SetProperty("open?", true);
                var otherSide = Portal.FindOppositeSide(this);
                otherSide.SetProperty("open?", true);
                return PerformResult.Continue;
            });

            Perform<MudObject, Hatch>("close").Do((a, b) =>
            {
                SetProperty("open?", false);
                var otherSide = Portal.FindOppositeSide(this);
                otherSide.SetProperty("open?", false);
                return PerformResult.Continue;
            });

            ControlPanel = new Space.ControlPanel();
            Move(ControlPanel, this);

            Check<Player, Hatch>("can open?")
                .Do((player, hatch) =>
                {
                    var thisSide = hatch.Location as Room;
                    var otherSide = Portal.FindOppositeSide(this).Location as Room;

                    if (!Game.SuitRepaired)
                    {
                        if (thisSide.AirLevel == AirLevel.Vacuum || otherSide.AirLevel == AirLevel.Vacuum)
                        {
                            SendMessage(player, "That would let all the air out. That's not a good idea with this hole in my suit.");
                            return CheckResult.Disallow;
                        }
                    }

                    if (ControlPanel.Broken) return CheckResult.Continue;

                    if (thisSide.AirLevel != otherSide.AirLevel)
                    {
                        SendMessage(player, "It won't open.");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                });
        }
    }
}
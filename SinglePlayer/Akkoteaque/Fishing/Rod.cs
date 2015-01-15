using RMUD;

namespace Akkoteaque.Fishing
{
    public class Rod : Container
    {
        public Rod()
            : base(RelativeLocations.On, RelativeLocations.On)
        { }

        public override void Initialize()
        {
            Short = "fishing rod";
            Nouns.Add("fishing", "rod");
            Long = "This is a compact, collapsible fishing rod.";

            Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .When((actor, item, container, location) => container == this && !(item is Hook) && !(item is Bait))
                .Do((actor, item, container, location) =>
                {
                    SendMessage(actor, "There doesn't seem to be any way to attach that to the fishing rod.");
                    return CheckResult.Disallow;
                });

            Perform<MudObject, MudObject, MudObject, RelativeLocations>("put")
                .When((actor, item, container, location) => container == this)
                .Do((actor, item, container, location) =>
                {
                    if (item is Hook)
                        this.RemoveAll(o => true);
                    else if (item is Bait)
                        this.RemoveAll(o => o is Bait);

                    this.Add(item, RelativeLocations.On);

                    if (item is Hook)
                        SendMessage(actor, "You tie <the0> to the line.", item);
                    else
                        SendMessage(actor, "You put <the0> on the hook.", item);

                    return PerformResult.Stop;
                });
        }
    }   
}
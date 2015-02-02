using RMUD;

namespace Space
{
    public class Locker : Container
    {
        public Locker() : base(RelativeLocations.In, RelativeLocations.In)
        {
            Open = false;
            Nouns.Add("LOCKER");
            Short = "locker";
            var seen = false;

            Check<MudObject, Locker>("can take?")
                .Do((actor, locker) =>
                   {
                       SendMessage(actor, "I think it's attached to the wall.");
                       return CheckResult.Disallow;
                   });

            Perform<Player, Locker>("describe in locale")
                .When((player, locker) => seen == false)
                .Do((player, locker) =>
                {
                    seen = true;
                    SendMessage(player, "There is a locker against the wall.");
                    return PerformResult.Stop;
                });

            Value<MudObject, bool>("openable?").Do(a => true);
            Value<MudObject, bool>("open?").Do(a => Open);

            Check<MudObject, MudObject>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (Open)
                    {
                        MudObject.SendMessage(a, "It's already open.");
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
                        MudObject.SendMessage(a, "It's already closed.");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                });
            
            Perform<MudObject, MudObject>("opened").Do((a, b) =>
            {
                Open = true;
                return PerformResult.Continue;
            });

            Perform<MudObject, MudObject>("closed").Do((a, b) =>
            {
                Open = false;
                return PerformResult.Continue;
            });
        }

        public bool Open { get; set; }
    }
}
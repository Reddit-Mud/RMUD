using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BasicDoor : Portal
    {
        public BasicDoor()
        {
            this.Nouns.Add("DOOR");
            this.Nouns.Add("CLOSED", actor => !Open);
            this.Nouns.Add("OPEN", actor => Open);
            Open = false;

            Value<MudObject, bool>("openable?").Do(a => true);
            Value<MudObject, bool>("open?").Do(a => Open);

            Check<MudObject, MudObject>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (Open)
                    {
                        MudObject.SendMessage(a, "It is already open.");
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
                        MudObject.SendMessage(a, "It is already closed.");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                });
            
            Perform<MudObject, MudObject>("opened").Do((a, b) =>
            {
                Open = true;

                var location = a.Location as Room;
                var otherSide = this.OppositeSide(location);
                if (otherSide != null)
                {
                    MudObject.SendLocaleMessage(otherSide as Room, "<a0> opens <the1>.", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });

            Perform<MudObject, MudObject>("closed").Do((a, b) =>
            {
                Open = false;

                var location = a.Location as Room;
                var otherSide = this.OppositeSide(location);
                if (otherSide != null)
                {
                    MudObject.SendLocaleMessage(otherSide as Room, "<a0> closes <the1>.", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });
        }

        public bool Open { get; set; }

    }
}

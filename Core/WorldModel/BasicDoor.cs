using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BasicDoor : MudObject
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
                if (otherSide != null)
                {
                    if (otherSide is BasicDoor) (otherSide as BasicDoor).Open = true;
                    MudObject.SendLocaleMessage(otherSide, "@they open", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });

            Perform<MudObject, MudObject>("closed").Do((a, b) =>
            {
                Open = false;

                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide != null)
                {
                    if (otherSide is BasicDoor) (otherSide as BasicDoor).Open = false;
                    MudObject.SendLocaleMessage(otherSide, "@they close", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });
        }

        public bool Open { get; set; }

    }
}

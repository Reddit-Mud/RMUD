using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

namespace RMUD
{
    public class BasicDoor : MudObject
    {
        public BasicDoor()
        {
            this.Nouns.Add("DOOR");
            this.Nouns.Add("CLOSED", actor => !GetBooleanProperty("open?"));
            this.Nouns.Add("OPEN", actor => GetBooleanProperty("open?"));

            SetProperty("open?", false);
            SetProperty("openable?", true);

            Check<MudObject, MudObject>("can open?")
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

            Check<MudObject, MudObject>("can close?")
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
            
            Perform<MudObject, MudObject>("opened").Do((a, b) =>
            {
                SetProperty("open?", true);

                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide != null)
                {
                    otherSide.SetProperty("open?", true);
                    MudObject.SendLocaleMessage(otherSide, "@they open", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });

            Perform<MudObject, MudObject>("closed").Do((a, b) =>
            {
                SetProperty("open?", false);

                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide != null)
                {
                    otherSide.SetProperty("open?", false);
                    MudObject.SendLocaleMessage(otherSide, "@they close", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            });
        }
    }
}

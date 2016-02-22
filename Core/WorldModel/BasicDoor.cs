using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRuleEngine;

namespace RMUD
{
    /// <summary>
    /// A basic door object. Doors are openable. When used as a portal, a door will automatically sync it's open state
    /// with the opposite side of the portal.
    /// </summary>
    public class BasicDoor : MudObject
    {
        public BasicDoor()
        {
            GetProperty<NounList>("nouns").Add("DOOR");

            // Doors can be referred to as 'the open door' or 'the closed door' as appropriate.
            GetProperty<NounList>("nouns").Add("CLOSED", actor => !GetPropertyOrDefault<bool>("open?"));
            GetProperty<NounList>("nouns").Add("OPEN", actor => GetPropertyOrDefault<bool>("open?"));

            SetProperty("open?", false);
            SetProperty("openable?", true);

            Check<MudObject, MudObject>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (GetPropertyOrDefault<bool>("open?"))
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
                    if (!GetPropertyOrDefault<bool>("open?"))
                    {
                        MudObject.SendMessage(a, "@already closed");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                })
                .Name("Can close doors rule.");

            Perform<MudObject, MudObject>("opened").Do((a, b) =>
            {
                SetProperty("open?", true);

                // Doors are usually two-sided. If there is an opposite side, we need to open it and emit appropriate
                // messages.
                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide != null)
                {
                    otherSide.SetProperty("open?", true);
                    
                    // This message is defined in the standard actions module. It is perhaps a bit coupled?
                    MudObject.SendLocaleMessage(otherSide, "@they open", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            })
            .Name("Open a door rule");

            Perform<MudObject, MudObject>("close").Do((a, b) =>
            {
                SetProperty("open?", false);

                // Doors are usually two-sided. If there is an opposite side, we need to close it and emit
                // appropriate messages.
                var otherSide = Portal.FindOppositeSide(this);
                if (otherSide != null)
                {
                    otherSide.SetProperty("open?", false);
                    MudObject.SendLocaleMessage(otherSide, "@they close", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            })
            .Name("Close a door rule");
        }
    }
}

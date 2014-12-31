using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        public static bool IsVisibleTo(MudObject Actor, MudObject Object)
        {
            var ceilingActor = MudObject.FindLocale(Actor);
            if (ceilingActor == null) return false;

            if (Object is Portal)
            {
                var ceilingA = MudObject.FindLocale((Object as Portal).FrontSide);
                var ceilingB = MudObject.FindLocale((Object as Portal).BackSide);

                return System.Object.ReferenceEquals(ceilingActor, ceilingA) || System.Object.ReferenceEquals(ceilingActor, ceilingB);
            }
            else
            {
                var ceilingObject = MudObject.FindLocale(Object);
                return System.Object.ReferenceEquals(ceilingActor, ceilingObject);
            }
        }

        public static CheckResult CheckIsVisibleTo(MudObject Actor, MudObject Item)
        {
            if (!MudObject.IsVisibleTo(Actor, Item))
            {
                MudObject.SendMessage(Actor, "That doesn't seem to be here any more.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }

        public static CheckResult CheckIsHolding(MudObject Actor, MudObject Item)
        {
            if (!MudObject.ObjectContainsObject(Actor, Item))
            {
                MudObject.SendMessage(Actor, "You don't have that.");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }
    }
}

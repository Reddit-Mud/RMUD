using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        public static MudObject FindLocale(MudObject Of)
        {
            if (Of.Location == null) return Of;

            var container = Of.Location as Container;
            if (container != null)
            {
                var relloc = container.RelativeLocationOf(Of);
                if (relloc == RelativeLocations.In) //Consider the open rules.
                {
                    if (GlobalRules.ConsiderValueRule<bool>("open?", Of.Location))
                        return FindLocale(Of.Location);
                    else 
                        return Of.Location;
                }
            }

            return FindLocale(Of.Location);
        }
    }
}

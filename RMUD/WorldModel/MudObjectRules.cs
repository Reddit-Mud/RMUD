using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class MudObjectRules 
    {
        public static void AtStartup()
        {
            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, String, String>("printed name", "[Viewer, Object, Article -> String] : Find the name that should be displayed for an object.");

            GlobalRules.Value<MudObject, MudObject, String, String>("printed name")
               .Last
               .Do((viewer, thing, article) => article + " " + thing.Short)
               .Name("Default name of a thing.");
        }
    }
}

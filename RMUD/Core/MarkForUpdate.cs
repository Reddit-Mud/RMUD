using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class UpdateRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject>("update", "[Thing] : Considered for all things that have been marked for update.", "item");

            GlobalRules.Perform<Actor>("after every command")
                .First
                .Do((actor) =>
                    {
                        Core.UpdateMarkedObjects();
                        return PerformResult.Continue;
                    })
                .Name("Update marked objects at end of turn rule.");

            GlobalRules.Perform<Actor>("after every command")
                .Last
                .Do((actor) =>
                    {
                        Core.SendPendingMessages();
                        return PerformResult.Continue;
                    })
               .Name("Send pending messages at end of turn rule.");
        }
    }

    public static partial class Core
    {
       internal static List<MudObject> MarkedObjects = new List<MudObject>();
		
        internal static void MarkLocaleForUpdate(MudObject Object)
        {
            MudObject locale = MudObject.FindLocale(Object);
            if (locale != null && !MarkedObjects.Contains(locale))
                MarkedObjects.Add(locale);
        }

        internal static void UpdateMarkedObjects()
        {
            var startCount = MarkedObjects.Count;
            for (int i = 0; i < startCount; ++i)
                GlobalRules.ConsiderPerformRule("update", MarkedObjects[i]);
            MarkedObjects.RemoveRange(0, startCount);
        }
    }

    public partial class MudObject
    {
        public static void MarkLocaleForUpdate(MudObject Object)
        {
            Core.MarkLocaleForUpdate(Object);
        }
    }
}

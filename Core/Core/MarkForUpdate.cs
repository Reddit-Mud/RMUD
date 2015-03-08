using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using SharpRuleEngine;

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
        private static List<MudObject> MarkedObjects = new List<MudObject>();

        /// <summary>
        /// Find the locale of an object and mark it for update.
        /// </summary>
        /// <param name="Object">Reference point object</param>
        public static void MarkLocaleForUpdate(MudObject Object)
        {
            MudObject locale = MudObject.FindLocale(Object);
            if (locale != null && !MarkedObjects.Contains(locale))
                MarkedObjects.Add(locale);
        }

        /// <summary>
        /// Run update rule on all objects that have been marked.
        /// </summary>
        public static void UpdateMarkedObjects()
        {
            // Updating an object may mark further objects for update. Avoid an infinite loop.
            var startCount = MarkedObjects.Count;
            for (int i = 0; i < startCount; ++i)
                GlobalRules.ConsiderPerformRule("update", MarkedObjects[i]);
            MarkedObjects.RemoveRange(0, startCount);
        }
    }
}

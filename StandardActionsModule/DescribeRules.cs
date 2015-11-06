using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;
using SharpRuleEngine;

namespace StandardActionsModule
{
    internal static class DescribeRules
    {
        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            Core.StandardMessage("is open", "^<the0> is open.");
            Core.StandardMessage("is closed", "^<the0> is closed.");
            Core.StandardMessage("describe on", "On <the0> is <l1>.");
            Core.StandardMessage("describe in", "In <the0> is <l1>.");
            Core.StandardMessage("empty handed", "^<the0> is empty handed.");
            Core.StandardMessage("holding", "^<the0> is holding <l1>.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe", "[Actor, Item] : Generates descriptions of the item.", "actor", "item");
                 
            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => !String.IsNullOrEmpty(item.Long))
                .Do((viewer, item) =>
                {
                    MudObject.SendMessage(viewer, item.Long);
                    return PerformResult.Continue;
                })
                .Name("Basic description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => item.GetBooleanProperty("openable?"))
                .Do((viewer, item) =>
                {
                    if (item.GetBooleanProperty("open?"))
                        MudObject.SendMessage(viewer, "@is open", item);
                    else
                        MudObject.SendMessage(viewer, "@is closed", item);
                    return PerformResult.Continue;
                })
                .Name("Describe open or closed state rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => (item is Container) && ((item as Container).LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                .Do((viewer, item) =>
                {
                    var contents = (item as Container).GetContents(RelativeLocations.On);
                    if (contents.Count() > 0)
                        MudObject.SendMessage(viewer, "@describe on", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things on container in description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) =>
                    {
                        if (!(item is Container)) return false;
                        if (!item.GetBooleanProperty("open?")) return false;
                        if ((item as Container).EnumerateObjects(RelativeLocations.In).Count() == 0) return false;
                        return true;
                    })
                .Do((viewer, item) =>
                {
                    var contents = (item as Container).GetContents(RelativeLocations.In);
                    if (contents.Count() > 0)
                        MudObject.SendMessage(viewer, "@describe in", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things in open container in description rule.");


            GlobalRules.Perform<MudObject, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var heldItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Held));
                    if (heldItems.Count == 0)
                        MudObject.SendMessage(viewer, "@empty handed", actor);
                    else
                        MudObject.SendMessage(viewer, "@holding", actor, heldItems);

                    return PerformResult.Continue;
                })
                .ID("list-actor-held-items-rule")
                .Name("List held items when describing an actor rule.");
        }

    }

    public static class DescribeExtensions
    {
        /// <summary>
        /// Factory that creates a Describe rule that applies only to the object it is called on.
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static RuleBuilder<MudObject, MudObject, PerformResult> PerformDescribe(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject>("describe").ThisOnly(1);
        }

        /// <summary>
        /// Adds a do clause that sends the message to the first argument, and then stops the action.
        /// </summary>
        /// <param name="RuleBuilder"></param>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static RuleBuilder<MudObject, MudObject, PerformResult> DoSimpleDescription(this RuleBuilder<MudObject, MudObject, PerformResult> RuleBuilder, String Str)
        {
            return RuleBuilder.Do((viewer, thing) =>
            {
                MudObject.SendMessage(viewer, Str);
                return PerformResult.Stop;
            });
        }
    }
}

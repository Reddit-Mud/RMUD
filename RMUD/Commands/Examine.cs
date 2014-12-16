using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Examine : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("EXAMINE", false),
                        new KeyWord("X", false)),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.")),
                new ExamineProcessor(),
                "Look closely at an object.");

            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("LOOK", false),
                        new KeyWord("L", false)),
                    new KeyWord("AT", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.")),
                new ExamineProcessor(),
                "Look closely at an object.");


        }

        public void InitializeGlobalRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can-examine", "[viewer, item] : Can the viewer examine the item?");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe", "[viewer, item] : Generates descriptions of the item.");

            GlobalRules.Check<MudObject, MudObject>("can-examine")
                .First
                .Do((viewer, item) => GlobalRules.IsVisibleTo(viewer, item))
                .Name("Can't examine what isn't here rule.");

            GlobalRules.Check<MudObject, MudObject>("can-examine")
                .Last
                .Do((viewer, item) => CheckResult.Allow)
                .Name("Default can examine everything rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => !String.IsNullOrEmpty(item.Long))
                .Do((viewer, item) =>
                {
                    Mud.SendMessage(viewer, item.Long);
                    return PerformResult.Continue;
                })
                .Name("Basic description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => GlobalRules.ConsiderValueRule<bool>("openable", item, item))
                .Do((viewer, item) =>
                {
                    if (GlobalRules.ConsiderValueRule<bool>("is-open", item, item))
                        Mud.SendMessage(viewer, "^<the0> is open.", item);
                    else
                        Mud.SendMessage(viewer, "^<the0> is closed.", item);
                    return PerformResult.Continue;
                })
                .Name("Describe open or closed state rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => (item is Container) && ((item as Container).LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                .Do((viewer, item) =>
                {
                    var contents = Mud.GetContents(item as Container, RelativeLocations.On);
                    if (contents.Count() > 0)
                    {
                        contents.Insert(0, item);
                        Mud.SendMessage(viewer, "On <the0> is " + Mud.UnformattedItemList(1, contents.Count - 1) + ".", contents.ToArray());
                    }
                    return PerformResult.Continue;
                })
                .Name("List things on container in description rule.");

            GlobalRules.Perform<MudObject, MudObject>("describe")
                .When((viewer, item) => Mud.HasVisibleContents(item))
                .Do((viewer, item) =>
                {
                    var contents = Mud.GetContents(item as Container, RelativeLocations.In);
                    if (contents.Count() > 0)
                    {
                        contents.Insert(0, item);
                        Mud.SendMessage(viewer, "In <the0> is " + Mud.UnformattedItemList(1, contents.Count - 1) + ".", contents.ToArray());
                    }
                    return PerformResult.Continue;
                })
                .Name("List things in open container in description rule.");
        }
    }

	internal class ExamineProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            var target = Match.Arguments["OBJECT"] as MudObject;

            if (GlobalRules.ConsiderCheckRule("can-examine", target, Actor, target) == CheckResult.Allow)
                GlobalRules.ConsiderPerformRule("describe", target, Actor, target);
        }
	}
}

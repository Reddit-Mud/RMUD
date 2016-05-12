using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AdminModule
{
	internal class Inspect : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!INSPECT"),
                    MustMatch("I don't see that here.",
                        Or(
                            Object("OBJECT", InScope),
                            KeyWord("HERE")))))
                .Manual("Take a peek at the internal workings of any mud object.")
                .ProceduralRule((match, actor) =>
                    {
                        if (!match.ContainsKey("OBJECT"))
                            match.Upsert("OBJECT", actor.Location);
                        return SharpRuleEngine.PerformResult.Continue;
                    }, "Convert locale option to standard form rule.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match["OBJECT"] as MudObject;

                    MudObject.SendMessage(actor, "*** INSPECT LISTING ***");
                    MudObject.SendMessage(actor, "Path: <s0>", target.Path);
                    MudObject.SendMessage(actor, "Instance: <s0>", target.Instance);
                    MudObject.SendMessage(actor, "Persistent: <s0>", target.IsPersistent.ToString());
                    if (target.Location == null)
                        MudObject.SendMessage(actor, "Location: NOWHERE");
                    else
                        MudObject.SendMessage(actor, "Location: <s0>", target.Location.GetFullName());
                    MudObject.SendMessage(actor, "*** DYNAMIC PROPERTIES ***");

                    foreach (var property in target.Properties)
                    {
                        var info = PropertyManifest.GetPropertyInformation(property.Key);
                        MudObject.SendMessage(actor, "<s0>: <s1>", property.Key, info.Converter.ConvertToString(property.Value));
                    }

                    MudObject.SendMessage(actor, "*** END OF LISTING ***");

                    return SharpRuleEngine.PerformResult.Continue;
                }, "List all the damn things rule.");
        }
	}
}

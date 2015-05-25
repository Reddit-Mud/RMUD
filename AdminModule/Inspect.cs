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
                    MudObject.SendMessage(actor, target.GetType().Name);

                    foreach (var @interface in target.GetType().GetInterfaces())
                        MudObject.SendMessage(actor, "Implements " + @interface.Name);

                    foreach (var field in target.GetType().GetFields())
                        MudObject.SendMessage(actor, "field " + field.FieldType.Name + " " + field.Name + " = " + WriteValue(field.GetValue(target)));

                    foreach (var property in target.GetType().GetProperties())
                    {
                        var s = (property.CanWrite ? "property " : "readonly ") + property.PropertyType.Name + " " + property.Name;
                        if (property.CanRead)
                        {
                            s += " = ";
                            try
                            {
                                s += WriteValue(property.GetValue(target, null));
                            }
                            catch (Exception) { s += "[Error reading value]"; }
                        }
                        MudObject.SendMessage(actor, s);
                    }

                    return SharpRuleEngine.PerformResult.Continue;
                }, "List all the damn things rule.");
        }

        private static String WriteValue(Object Value, int indent = 1)
        {
            if (Value == null)
                return "NULL";
            else if (Value is String)
                return "\"" + Value + "\"";
            else if (Value is MudObject)
            {
                return (Value as MudObject).GetFullName();
            }
            else if (Value is KeyValuePair<String, Object>)
            {
                var v = (Value as KeyValuePair<String, Object>?).Value;
                return v.Key + ": " + WriteValue(v.Value, indent + 1);
            }
            else if (Value is KeyValuePair<RelativeLocations, List<MudObject>>) //Containers..
            {
                var v = (Value as KeyValuePair<RelativeLocations, List<MudObject>>?).Value;
                return v.Key + ": " + WriteValue(v.Value, indent + 1);
            }
            else if (Value is System.Collections.IEnumerable)
            {
                var r = "[ ";
                bool first = true;
                foreach (var sub in (Value as System.Collections.IEnumerable))
                {
                    if (!first) r += "\n" + new String(' ', indent * 2);
                    first = false;
                    r += WriteValue(sub, indent + 1);
                }
                r += " ] ";
                return r;
            }
            else return Value.ToString();
        }
	}
}

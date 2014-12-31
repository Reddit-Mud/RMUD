using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
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
                        return PerformResult.Continue;
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

                    return PerformResult.Continue;
                }, "List all the damn things rule.");
        }

        private static String WriteValue(Object Value)
        {
            if (Value == null)
                return "NULL";
            else if (Value is String)
                return "\"" + Value + "\"";
            else if (Value is MudObject)
                return Value.ToString();
            else if (Value is System.Collections.IEnumerable)
            {
                var r = "[ ";
                foreach (var sub in (Value as System.Collections.IEnumerable))
                    r += WriteValue(sub + ", ");
                if (r.Length > 2) r = r.Remove(r.Length - 2, 2);
                r += " ]";
                return r;
            }
            else return Value.ToString();
        }
	}
}

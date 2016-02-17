using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace AdminModule
{
    internal class Set : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    RequiredRank(500),
                    KeyWord("!SET"),
                    MustMatch("I don't see that here.",
                        Object("OBJECT", InScope)),
                    SingleWord("PROPERTY"),
                    Rest("VALUE")))
                .Manual("Set the value of a property on an object.")
                .ProceduralRule((match, actor) =>
                {
                    var _object = match["OBJECT"] as MudObject;
                    var property_name = match["PROPERTY"].ToString();
                    var value = match["VALUE"].ToString();

                    var property_info = _object.GetType().GetProperty(property_name);
                    if (property_info == null)
                    {
                        MudObject.SendMessage(actor, "I could not find that property on that object.");
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    if (!property_info.CanWrite)
                    {
                        MudObject.SendMessage(actor, "That property is read-only.");
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    try
                    {
                        var convertedValue = Convert.ChangeType(value, property_info.PropertyType);
                        property_info.SetValue(_object, convertedValue);
                    }
                    catch (Exception e)
                    {
                        MudObject.SendMessage(actor, "There was an error while attempting to assign that property: " + e.Message);
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    MudObject.SendMessage(actor, "Property set.");
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}
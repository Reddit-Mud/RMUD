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
                    var stringValue = match["VALUE"].ToString();

                    var propertyInfo = PropertyManifest.GetPropertyInformation(property_name);

                    if (propertyInfo == null)
                    {
                        MudObject.SendMessage(actor, "That property does not exist.");
                        return SharpRuleEngine.PerformResult.Stop;
                    }

                    var realValue = propertyInfo.Converter.ConvertFromString(stringValue);

                    _object.SetProperty(property_name, realValue);
                    
                    MudObject.SendMessage(actor, "Property set.");
                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}
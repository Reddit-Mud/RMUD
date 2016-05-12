using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace DelmudGameplay
{
    public class CombatStats : RMUD.CommandFactory
    {
        public static void AtStartup()
        {
            RMUD.PropertyManifest.RegisterProperty("strength", typeof(int), 0, new IntSerializer());
            RMUD.PropertyManifest.RegisterProperty("accuracy", typeof(int), 0, new IntSerializer());
            RMUD.PropertyManifest.RegisterProperty("power", typeof(int), 0, new IntSerializer());

            RMUD.PropertyManifest.RegisterProperty("class", typeof(CharacterClasses), CharacterClasses.Bludgeon, new EnumSerializer<CharacterClasses>());
            RMUD.PropertyManifest.RegisterProperty("element", typeof(ElementTypes), ElementTypes.Earth, new EnumSerializer<ElementTypes>());

            RMUD.PropertyManifest.RegisterProperty("max-hp", typeof(int), 0, new IntSerializer());
            RMUD.PropertyManifest.RegisterProperty("max-mp", typeof(int), 0, new IntSerializer());
            RMUD.PropertyManifest.RegisterProperty("current-hp", typeof(int), 0, new IntSerializer());
            RMUD.PropertyManifest.RegisterProperty("current-mp", typeof(int), 0, new IntSerializer());
        }

        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("SCORE")))
                .ID("Delmud:Score")
                .Manual("Display stats about the current player.")
                .ProceduralRule((match, actor) =>
                {
                    MudObject.SendMessage(actor, "Your stats :");
                    MudObject.SendMessage(actor, "Strength: <s0>", actor.GetProperty<int>("strength"));
                    MudObject.SendMessage(actor, "Accuracy: <s0>", actor.GetProperty<int>("accuracy"));
                    MudObject.SendMessage(actor, "Power: <s0>", actor.GetProperty<int>("power"));
                    MudObject.SendMessage(actor, "Class: <s0>", actor.GetProperty<CharacterClasses>("class"));
                    MudObject.SendMessage(actor, "Element: <s0>", actor.GetProperty<ElementTypes>("element"));
                    MudObject.SendMessage(actor, "HP: <s0> / <s1>", actor.GetProperty<int>("current-hp"), actor.GetProperty<int>("max-hp"));
                    MudObject.SendMessage(actor, "MP: <s0> / <s1>", actor.GetProperty<int>("current-mp"), actor.GetProperty<int>("max-mp"));

                    return SharpRuleEngine.PerformResult.Continue;
                });
        }
    }
}

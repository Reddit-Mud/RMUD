using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimum
{
    public static class Game
    {
        public static RMUD.SinglePlayer.Driver Driver { get; set; }
        internal static RMUD.Player Player { get { return Driver.Player; } }

        public static void SwitchPlayerCharacter(RMUD.Player NewCharacter)
        {
            Driver.SwitchPlayerCharacter(NewCharacter);
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<Player>("singleplayer game started")
                .First
                .Do((actor) =>
                {
                    SwitchPlayerCharacter(RMUD.MudObject.GetObject("Player") as RMUD.Player);
                    RMUD.MudObject.Move(Player, RMUD.MudObject.GetObject("Start"));
                    RMUD.Core.EnqueuActorCommand(Player, "look");
        
                    return SharpRuleEngine.PerformResult.Stop;
                });
        }
    }
}
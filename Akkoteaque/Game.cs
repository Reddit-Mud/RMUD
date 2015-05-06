using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akko
{

    public static class Game
    {
        internal static bool BlockingConversation = false;
        internal static bool SuppressTopics = false;
        internal static bool SuitRepaired = false;
        public static RMUD.SinglePlayer.Driver Driver { get; set; }
        internal static RMUD.Player Player { get { return Driver.Player; } }

        public static void SwitchPlayerCharacter(RMUD.Player NewCharacter)
        {
            Driver.SwitchPlayerCharacter(NewCharacter);
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            ConversationModule.Settings.ListDiscussedTopics = false;
            ConversationModule.Settings.AllowRepeats = false;

            RMUD.Core.OverrideMessage("empty handed", "");
            RMUD.Core.OverrideMessage("convo topic prompt", "You could ask <l0>.");

            GlobalRules.Perform<Player>("list topics")
                .When(player => SuppressTopics)
                .Do(player => RMUD.PerformResult.Stop);

            GlobalRules.Perform<Player>("singleplayer game started")
                .First
                .Do((actor) =>
                {
                    //BlockingConversation = true;

                    //RMUD.MudObject.SendMessage(actor, "Sal? Sal? Can you hear me?");
                    //actor.SetProperty("interlocutor", RMUD.MudObject.GetObject("DanConversation0"));
                    //RMUD.Core.EnqueuActorCommand(actor, "topics");

                    SwitchPlayerCharacter(RMUD.MudObject.GetObject("Areas.Prologue.Player") as RMUD.Player);
                    RMUD.MudObject.Move(Player, RMUD.MudObject.GetObject("Areas.Prologue.Car"));
                    RMUD.Core.EnqueuActorCommand(Player, "look");
                    
                    //Player.SetProperty("interlocutor", RMUD.MudObject.GetObject("Areas.Prologue.Henrico"));
                    //RMUD.Core.EnqueuActorCommand(Player, "topics");
                    //BlockingConversation = true;
                    
                    return RMUD.PerformResult.Stop;
                });
        }
    }
}
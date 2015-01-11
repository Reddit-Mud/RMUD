using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.SinglePlayer
{
    public class Driver
    {
        private DummyClient Client;
        private RMUD.Player Player;
        public bool BlockOnInput { get; set; }
        private System.Threading.AutoResetEvent CommandQueueReady = new System.Threading.AutoResetEvent(false);
        public bool IsRunning
        {
            get
            {
                return !RMUD.Core.ShuttingDown;
            }
        }

        public Driver()
        {
            BlockOnInput = true;
        }

        public bool Start(
            System.Reflection.Assembly DatabaseAssembly, 
            String ObjectNamespace,
            Action<String> Output)
        {
            if (RMUD.Core.Start(
                new RMUD.SinglePlayer.CompiledDatabase(DatabaseAssembly, ObjectNamespace),
                new RMUD.StartUpAssembly(DatabaseAssembly, ObjectNamespace)))
            {
                Player = RMUD.MudObject.GetObject<RMUD.Player>(RMUD.Core.SettingsObject.PlayerBaseObject);
                Player.CommandHandler = RMUD.Core.ParserCommandHandler;
                Client = new DummyClient(Output);
                RMUD.Core.TiePlayerToClient(Client, Player);
                RMUD.Core.AddPlayer(Player);

                return true;
            }

            return false;
        }

        public void Input(String Command)
        {
            if (BlockOnInput)
            {
                RMUD.Core.EnqueuActorCommand(Player, Command, () =>
                    CommandQueueReady.Set());
                CommandQueueReady.WaitOne();
            }
            else
                RMUD.Core.EnqueuActorCommand(Player, Command);
        }
    }
}

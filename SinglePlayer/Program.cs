using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinglePlayer
{
    class DummyClient : RMUD.Client
    {
        public override void Send(string message)
        {
            Console.Write(message);
        }

        public override void Disconnect()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (RMUD.Core.Start(new RMUD.SinglePlayer.CompiledDatabase(System.Reflection.Assembly.GetExecutingAssembly(), "SinglePlayer.Database"), System.Reflection.Assembly.GetExecutingAssembly()))
            {
                var playerObject = RMUD.Core.Database.GetObject("Player") as RMUD.Player;
                playerObject.CommandHandler = RMUD.Core.ParserCommandHandler;

                var client = new DummyClient();
                RMUD.Core.TiePlayerToClient(client, playerObject);
                RMUD.Core.AddPlayer(playerObject);

                bool playing = true;
                RMUD.Core.OnShutDown += () => playing = false;

                var commandQueuReady = new System.Threading.AutoResetEvent(false);

                while (playing)
                {
                    var input = Console.ReadLine();
                    RMUD.Core.EnqueuActorCommand(playerObject, input, () => commandQueuReady.Set());
                    commandQueuReady.WaitOne();
                }
            }
            else
            {
                Console.WriteLine("ERROR.");
            }

            Console.WriteLine("[Press any key to exit..]");
            Console.ReadKey();
        }
    }
}

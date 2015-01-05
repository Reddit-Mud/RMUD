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
            if (RMUD.Core.Start(new RMUD.GithubDatabase()))
            {
                var playerObject = RMUD.Core.Database.GetObject(RMUD.Core.SettingsObject.PlayerBaseObject + "@" + "player") as RMUD.Player;

                playerObject.Short = "Player";
                playerObject.Nouns.Add("PLAYER");
                playerObject.CommandHandler = RMUD.Core.ParserCommandHandler;

                var client = new DummyClient();
                RMUD.Core.TiePlayerToClient(client, playerObject);
                RMUD.Core.AddPlayer(playerObject);

                while (true)
                {
                    var input = Console.ReadLine();
                    RMUD.Core.EnqueuActorCommand(playerObject, input);
                }
            }
            else
            {
                Console.WriteLine("ERROR.");
                while (true) { }
            }
        }
    }
}

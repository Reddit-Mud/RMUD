using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            var demoGames = new List<String>();
            foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
                if (!demoGames.Contains(type.Namespace))
                    demoGames.Add(type.Namespace);
            foreach (var name in demoGames.Where(s => System.Reflection.Assembly.GetExecutingAssembly().GetType(s + ".settings", false) != null))
                Console.WriteLine(name);


            var driver = new RMUD.SinglePlayer.Driver();
            driver.Start(System.Reflection.Assembly.GetExecutingAssembly(), "CloakOfDarkness", Console.Write);
            while (driver.IsRunning)
                driver.Input(Console.ReadLine());

            Console.WriteLine("[Press any key to exit..]");
            Console.ReadKey();
        }
    }
}

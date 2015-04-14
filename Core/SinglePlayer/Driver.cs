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
            Action<String> Output,
            params StartUpAssembly[] AdditionalAssemblies)
        {
            var assemblies = new List<StartUpAssembly>();
            assemblies.Add(new StartUpAssembly(DatabaseAssembly, new ModuleInfo { BaseNameSpace = ObjectNamespace }));
            assemblies.AddRange(AdditionalAssemblies);

            if (RMUD.Core.Start(StartupFlags.Silent | StartupFlags.SearchDirectory,
                new RMUD.SinglePlayer.CompiledDatabase(DatabaseAssembly, ObjectNamespace),
                assemblies.ToArray()))
            {
                Player = RMUD.MudObject.GetObject<RMUD.Player>(RMUD.Core.SettingsObject.PlayerBaseObject);
                Player.CommandHandler = RMUD.Core.ParserCommandHandler;
                Client = new DummyClient(Output);
                RMUD.Core.TiePlayerToClient(Client, Player);
                Player.Rank = 500;
                Core.GlobalRules.ConsiderPerformRule("singleplayer game started", Player);

                return true;
            }

            return false;
        }


        public bool Start(
            String AssemblyFile,
            Action<String> Output)
        {
            var assembly = System.Reflection.Assembly.LoadFile(System.IO.Path.GetFullPath(AssemblyFile));
            if (assembly == null) throw new InvalidOperationException("Game assembly could not be loaded.");

            var gameInfo = assembly.CreateInstance("GameInfo") as RMUD.SinglePlayer.GameInfo;
            if (gameInfo == null) throw new InvalidOperationException("No GameInfo defined in game assembly.");

            var assemblies = new List<StartUpAssembly>();
            assemblies.Add(new StartUpAssembly(assembly, new ModuleInfo { BaseNameSpace = gameInfo.DatabaseNameSpace }, AssemblyFile));
            foreach (var module in gameInfo.Modules)
                assemblies.Add(new StartUpAssembly(module));

            if (RMUD.Core.Start(StartupFlags.Silent,
                new RMUD.SinglePlayer.CompiledDatabase(assembly, gameInfo.DatabaseNameSpace),
                assemblies.ToArray()))
            {
                Player = RMUD.MudObject.GetObject<RMUD.Player>(RMUD.Core.SettingsObject.PlayerBaseObject);
                Player.CommandHandler = RMUD.Core.ParserCommandHandler;
                Client = new DummyClient(Output);
                RMUD.Core.TiePlayerToClient(Client, Player);
                Player.Rank = 500;
                Core.GlobalRules.ConsiderPerformRule("singleplayer game started", Player);

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

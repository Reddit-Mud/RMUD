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
        public RMUD.Player Player { get; private set; }
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

        public void SwitchPlayerCharacter(RMUD.Player NewCharacter)
        {
            NewCharacter.Rank = 500;
            NewCharacter.CommandHandler = RMUD.Core.ParserCommandHandler;
            var client = Player.ConnectedClient;
            RMUD.Core.TiePlayerToClient(client, NewCharacter);
            Player = NewCharacter;
        }

        private void DetectAndAssignDriver(System.Reflection.Assembly Assembly)
        {
            foreach (var type in Assembly.GetTypes().Where(t => t.Name == "Game"))
            {
                try
                {
                    var property = type.GetProperty("Driver");
                    if (property != null && property.PropertyType == typeof(Driver) && property.CanWrite)
                        property.SetValue(null, this);
                }
                catch (Exception e) { }
            }
        }

        public bool Start(
            System.Reflection.Assembly DatabaseAssembly, 
            Action<String> Output)
        {
            GameInfo gameInfo = null;
            foreach (var type in DatabaseAssembly.GetTypes())
                if (type.IsSubclassOf(typeof(GameInfo)))
                    gameInfo = Activator.CreateInstance(type) as GameInfo;

            if (gameInfo == null) throw new InvalidOperationException("No GameInfo defined in game assembly.");

            var assemblies = new List<StartUpAssembly>();
            assemblies.Add(new StartUpAssembly(DatabaseAssembly, new ModuleInfo { BaseNameSpace = gameInfo.DatabaseNameSpace }));
            foreach (var module in gameInfo.Modules)
                assemblies.Add(new StartUpAssembly(module));

            if (RMUD.Core.Start(StartupFlags.Silent,
                new RMUD.SinglePlayer.CompiledDatabase(DatabaseAssembly, gameInfo.DatabaseNameSpace),
                assemblies.ToArray()))
            {
                Player = RMUD.MudObject.GetObject<RMUD.Player>(RMUD.Core.SettingsObject.PlayerBaseObject);
                Player.CommandHandler = RMUD.Core.ParserCommandHandler;
                Client = new DummyClient(Output);
                RMUD.Core.TiePlayerToClient(Client, Player);
                Player.Rank = 500;

                DetectAndAssignDriver(DatabaseAssembly);
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

            GameInfo gameInfo = null;
            foreach (var type in assembly.GetTypes())
                if (type.IsSubclassOf(typeof(GameInfo)))
                    gameInfo = Activator.CreateInstance(type) as GameInfo;

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

                DetectAndAssignDriver(assembly);
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

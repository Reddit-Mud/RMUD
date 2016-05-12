﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.SinglePlayer
{
    public class Driver
    {
        private DummyClient Client;
        public MudObject Player { get; private set; }
        
        public bool IsRunning
        {
            get
            {
                return !Core.ShuttingDown;
            }
        }

        public Driver()
        {
        }

        public void SwitchPlayerCharacter(MudObject NewCharacter)
        {
            //NewCharacter.Rank = 500;
            NewCharacter.SetProperty("command handler", Core.ParserCommandHandler);
            var client = Player.GetProperty<Client>("client");
            if (client != null)
                Core.TiePlayerToClient(client, NewCharacter);
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

            var assemblies = new List<ModuleAssembly>();
            foreach (var module in gameInfo.Modules)
                assemblies.Add(new ModuleAssembly(module));

            // Add the database assembly last, so that's AtStartup items are called last.
            assemblies.Add(new ModuleAssembly(DatabaseAssembly, new ModuleInfo { BaseNameSpace = gameInfo.DatabaseNameSpace }));

            if (RMUD.Core.Start(StartupFlags.Silent | StartupFlags.SingleThreaded,
                "database/",
                new RMUD.SinglePlayer.CompiledDatabase(DatabaseAssembly, gameInfo.DatabaseNameSpace),
                assemblies.ToArray()))
            {
                Player = RMUD.MudObject.GetObject(RMUD.Core.SettingsObject.PlayerBaseObject);
                Player.SetProperty("command handler", Core.ParserCommandHandler);
                Client = new DummyClient(Output);
                RMUD.Core.TiePlayerToClient(Client, Player);
                //Player.Rank = 500;

                DetectAndAssignDriver(DatabaseAssembly);
                Core.GlobalRules.ConsiderPerformRule("singleplayer game started", Player);

                return true;
            }

            return false;
        }

        public void Input(String Command)
        {
            RMUD.Core.EnqueuActorCommand(Player, Command);
            RMUD.Core.ProcessCommands();
            RMUD.Core.SendPendingMessages(); // This is a hack, in case messages are never sent.
        }
    }
}

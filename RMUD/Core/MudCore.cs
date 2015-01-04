using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Core
    {
        internal static Mutex DatabaseLock = new Mutex();
        private static bool ShuttingDown = false;

        internal static Settings SettingsObject;

        internal static WorldDataService Database;

        public static bool Start(WorldDataService Database)
        {
            try
            {
                InitializeCommandProcessor();
                GlobalRules.DiscoverRuleBooks(System.Reflection.Assembly.GetExecutingAssembly());
                InitializeStaticManPages();
                PersistentValueSerializer.AddGlobalSerializer(new BitArraySerializer());

                Core.Database = Database;
                Database.Initialize();

                foreach (var method in typeof(Core).GetMethods())
                    if (method.IsStatic && method.Name.StartsWith("InitializeModule_"))
                        method.Invoke(null, null);

                StartCommandProcesor();
            }
            catch (Exception e)
            {
                LogError("Failed to start mud engine.");
                LogError(e.Message);
                LogError(e.StackTrace);
                throw;
            }
            return true;
        }

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}

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
        private static String CriticalLog = "errors.log";
        public static Action<String> DynamicCriticalLog = Console.WriteLine;

        public static void LogCommandError(Exception e)
        {
            if (!Core.NoLog)
            {
                var logfile = new System.IO.StreamWriter(CriticalLog, true);
                logfile.WriteLine("{0:MM/dd/yy HH:mm:ss} -- Error while handling client command.", DateTime.Now);
                logfile.WriteLine(e.Message);
                logfile.WriteLine(e.StackTrace);
                logfile.Close();
                
                DynamicCriticalLog(String.Format("{0:MM/dd/yy HH:mm:ss} -- Error while handling client command.", DateTime.Now));
                DynamicCriticalLog(e.Message);
                DynamicCriticalLog(e.StackTrace);

                if (e.InnerException != null)
                    LogCriticalError(e.InnerException);
            }
        }

        public static void LogCriticalError(Exception e)
        {
            if (!Core.NoLog)
            {
                var logfile = new System.IO.StreamWriter(CriticalLog, true);
                logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- Critical error.", DateTime.Now);
                logfile.WriteLine(e.GetType().Name);
                logfile.WriteLine(e.Message);
                logfile.WriteLine(e.StackTrace);
                logfile.Close();

                DynamicCriticalLog(String.Format("{0:MM/dd/yy H:mm:ss} -- Critical error.", DateTime.Now));
                DynamicCriticalLog(e.GetType().Name);
                DynamicCriticalLog(e.Message);
                DynamicCriticalLog(e.StackTrace);

                if (e.InnerException != null)
                    LogCriticalError(e.InnerException);
                
                if (e is AggregateException)
                {
                    var ag = e as AggregateException;
                    foreach (var _e in ag.InnerExceptions)
                        LogCriticalError(_e);
                }
            }
        }

        public static void LogError(String ErrorString)
        {
            if (!Core.NoLog)
            {
                var logfile = new System.IO.StreamWriter(CriticalLog, true);
                logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- {1}\n", DateTime.Now, ErrorString);
                logfile.Close();

                DynamicCriticalLog(String.Format("{0:MM/dd/yy H:mm:ss} -- {1}\n", DateTime.Now, ErrorString));
            }
        }

        public static void LogWarning(String Warning)
        {
            if (!Core.NoLog)
            {
                var logfile = new System.IO.StreamWriter(CriticalLog, true);
                logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- WARNING: {1}", DateTime.Now, Warning);
                logfile.Close();

                DynamicCriticalLog(String.Format("{0:MM/dd/yy H:mm:ss} -- WARNING: {1}", DateTime.Now, Warning));
            }
        }
    }
}

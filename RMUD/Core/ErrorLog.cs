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

        public static void LogCommandError(Exception e)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- Error while handling client command.", DateTime.Now);
            logfile.WriteLine(e.Message);
            logfile.WriteLine(e.StackTrace);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- Error while handling client command.", DateTime.Now);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        public static void LogCriticalError(Exception e)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- Critical error.", DateTime.Now);
            logfile.WriteLine(e.Message);
            logfile.WriteLine(e.StackTrace);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- Critical error.", DateTime.Now);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        public static void LogError(String ErrorString)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- {1}\n", DateTime.Now, ErrorString);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- {1}\n", DateTime.Now, ErrorString);
        }

        public static void LogWarning(String Warning)
        {
            var logfile = new System.IO.StreamWriter(CriticalLog, true);
            logfile.WriteLine("{0:MM/dd/yy H:mm:ss} -- WARNING: {1}", DateTime.Now, Warning);
            logfile.Close();

            Console.WriteLine("{0:MM/dd/yy H:mm:ss} -- WARNING: {1}", DateTime.Now, Warning);
        }
    }
}

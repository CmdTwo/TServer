using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Generic
{
    public class Log
    {
        private static string Path = "\\log.txt";
        private static StreamWriter LogFile;

        public static void OpenLog()
        {
            LogFile = File.AppendText(Directory.GetCurrentDirectory() + Path);
        }

        public static void Write(string LogMessage)
        {
            Console.Write(LogMessage + " | ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(DateTime.Now + "\n");
            Console.ResetColor();

            LogFile.Write(LogMessage);
        }

        public static void CloseLog()
        {
            LogFile.Write("\r\n");
            LogFile.Close();
        }
    }
}

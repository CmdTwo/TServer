using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TServer
{
    public class ServerConsole
    {
        private enum Commands : byte
        {
            help = 1,
        }

        public ServerConsole()
        {
            WriteBreakLine(31);
            WriteColorLine("Testlo Server");
            WriteInfoLine("Author", "Bernikovich Alexey (c)");
            WriteBreakLine(31);
        }

        public void WriteBreakLine(int length = 30)
        {
            Console.Write(" ");
            for (int i = 0; i < length; i++)
                Console.Write("═");
            Console.WriteLine();
        }

        public void WriteColorLine(string text, ConsoleColor consoleColor = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(" " + text);
        }

        public void WriteInfoLine(string firstPart, string secondPart, char separator = '|', ConsoleColor firstColor = ConsoleColor.DarkCyan, ConsoleColor secondColor = ConsoleColor.White)
        {
            Console.ForegroundColor = firstColor;
            Console.Write(" " + firstPart);
            Console.ForegroundColor = secondColor;
            Console.WriteLine(" " + separator + " " + secondPart);
        }
        
        public void ExecuteCommand(string command)
        {
            // Выполнение команды
        }

        public void WriteLogMessage()
        {
            // Логи
        }

        public void StartInputCommands()
        {
            //Thread inputCommandsThread = new Thread( delegate() {
            //    while(true)
            //    {
            //        Console.Write
            //    }
            //});
        }
    }
}

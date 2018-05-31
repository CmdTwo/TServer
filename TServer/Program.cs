using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(Server.FindMyIp(), 25252);
            server.Launch();

            //ServerConsole serverConsole = new ServerConsole();
            Console.ReadKey();
        }
    }
}

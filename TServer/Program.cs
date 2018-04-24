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
            Server server = new Server("192.168.100.4", 25252);
            server.Launch();
            Console.ReadKey();
        }
    }
}

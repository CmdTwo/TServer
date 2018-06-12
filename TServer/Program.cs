using System;
using System.Collections.Generic;
using System.Globalization;
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

            //DateTime dateTime = DateTime.Now;
            //string date = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            //dateTime = DateTime.ParseExact(date, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
            ////ServerConsole serverConsole = new ServerConsole();
            Console.ReadKey();
        }
    }
}

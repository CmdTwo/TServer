using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using TServer.Generic;
using TServer.Common;


namespace TServer
{
    public class Server
    {
        private TcpListener Listener;
        private IPAddress ServerIP;
        private int ServerPort;
        private ManualResetEvent AllDone;
        private List<TClient> ClientsList;
        private DBManager DBManager;

        public Server(string ip, int port)
        {
            AllDone = new ManualResetEvent(false);
            ClientsList = new List<TClient>();

            ServerIP = IPAddress.Parse(ip);
            ServerPort = port;
            Listener = new TcpListener(ServerIP, ServerPort);
            DBManager = new DBManager();
        }

        public void Launch()
        {
            Listener.Start();

            Log.OpenLog();
            Log.Write("Server started! IP: " + ServerIP + ":" + ServerPort);            
            BeginAccept();
        }

        private void BeginAccept()
        {
            Thread AcceptThread = new Thread(delegate ()
            {
                while (true) ///////FIX
                {
                    AllDone.Reset();
                    Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), Listener);
                    AllDone.WaitOne();
                }
            });
            AcceptThread.IsBackground = true;
            AcceptThread.Start();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();
            Log.Write("New user connected!");
            TcpListener listener = (TcpListener)ar.AsyncState;
            TClient newClient;
            ClientsList.Add(newClient = new TClient(listener.EndAcceptTcpClient(ar), DBManager, this));
            BeginRead(newClient, ar);
        }
        
        public void BeginRead(TClient client, IAsyncResult ar)
        {
            StateObject state = new StateObject() { WorkSocket = client.TcpClient };
            Log.Write("Begin read...");
            client.TcpClient.GetStream().BeginRead(state.Buffer, 0, StateObject.BufferSize, new AsyncCallback(client.ReadCallback), state);
        }

        public static string FindMyIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            return "0.0.0.0";
        }
    }
}

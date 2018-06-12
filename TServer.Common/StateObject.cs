using System.Net.Sockets;

namespace TServer.Common
{
    public class StateObject
    {
        public TcpClient WorkSocket = null;
        public const int BufferSize = 36000;
        public byte[] Buffer = new byte[BufferSize];
    }
}

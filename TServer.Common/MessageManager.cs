using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common
{
    public class MessageManager
    {
        private BinaryFormatter BinaryFormatter;
        private MemoryStream MemoryStream;

        public MessageManager()
        {
            BinaryFormatter = new BinaryFormatter();
            MemoryStream = new MemoryStream();
        }

        public byte[] SerializeMessage<T>(Dictionary<T, object> dictionary)
        {
            MemoryStream = new MemoryStream();
            BinaryFormatter.Serialize(MemoryStream, dictionary);
            return MemoryStream.ToArray();
        }

        public Dictionary<T, object> DeserializeMessage<T>(byte[] bytes, int read)
        {
            MemoryStream.Write(bytes, 0, read);
            MemoryStream.Position = 0;
            return BinaryFormatter.Deserialize(MemoryStream) as Dictionary<T, object>;
        }
    }
}

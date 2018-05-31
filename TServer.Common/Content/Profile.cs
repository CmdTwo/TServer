using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class Profile
    {
        public int ID { get; }
        public string Name { get; }

        public Profile()
        {

        }

        public Profile(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}

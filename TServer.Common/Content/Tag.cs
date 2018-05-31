using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class Tag : IContent
    {
        public int ID { get; }
        public string Name { get; }

        public Tag(int id, string name)
        {
            ID = id;
            Name = name;
        }
        public object GetField(ContentParam param)
        {
            switch (param)
            {
                case (ContentParam.ID):
                    return ID;
                case (ContentParam.name):
                    return Name;
            }
            return null;
        }

        public class TagComparer : IEqualityComparer<Tag>
        { 
            public bool Equals(Tag x, Tag y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.ID == y.ID && x.Name == y.Name;
            }

            public int GetHashCode(Tag tag)
            {
                if (Object.ReferenceEquals(tag, null)) return 0;
                int hashProductName = tag.ID == null ? 0 : tag.Name.GetHashCode();
                int hashProductCode = tag.Name.GetHashCode();
                return hashProductName ^ hashProductCode;
            }
        }
    }
}

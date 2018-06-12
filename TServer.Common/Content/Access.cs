using System;
using System.Collections;
using System.Collections.Generic;

namespace TServer.Common.Content
{
    [Serializable]
    public class Access : IContent
    {
        public int ID { get; }
        public string Name { get; }
        public string FaIcon { get; }
        public List<SubAccess> SubAccesses;

        public Access(int id, string name, string faIcon)
        {
            ID = id;
            Name = name;
            FaIcon = faIcon;
            SubAccesses = new List<SubAccess>();
        }

        public Access( string name, string faIcon)
        {
            Name = name;
            FaIcon = faIcon;
            SubAccesses = new List<SubAccess>();
        }

        public object GetField(ContentParam param)
        {
            switch(param)
            {
                case (ContentParam.ID):
                    return ID;
                case (ContentParam.name):
                    return Name;
                case (ContentParam.faIcon):
                    return FaIcon;
            }
            return null;
        }

        public void SetAccesses(List<SubAccess> value)
        {
            SubAccesses = value;
        }

        public class AccessComparer : IEqualityComparer<Access>
        {
            public bool Equals(Access x, Access y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.ID == y.ID && x.Name == y.Name;
            }

            public int GetHashCode(Access access)
            {
                if (Object.ReferenceEquals(access, null)) return 0;
                int hashProductName = access.ID == null ? 0 : access.Name.GetHashCode();
                int hashProductCode = access.Name.GetHashCode();
                return hashProductName ^ hashProductCode;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;


namespace TServer.Common.Content
{
    [Serializable]
    public class SubAccess : IContent
    {
        public int ID { get; }
        public string Name { get; }

        public SubAccess(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public SubAccess(string name)
        {
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

        public class SubAccessComparer : IEqualityComparer<SubAccess>
        {
            public bool Equals(SubAccess x, SubAccess y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.ID == y.ID && x.Name == y.Name;
            }

            public int GetHashCode(SubAccess subAccess)
            {
                if (Object.ReferenceEquals(subAccess, null)) return 0;
                int hashProductName = subAccess.ID == null ? 0 : subAccess.Name.GetHashCode();
                int hashProductCode = subAccess.Name.GetHashCode();
                return hashProductName ^ hashProductCode;
            }
        }
    }
}

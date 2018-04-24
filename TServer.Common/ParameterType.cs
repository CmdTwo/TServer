using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common
{
    public enum ParameterType:byte
    {
        login = 1,
        password = 2,
        authorized = 3,
        name = 4,
        groupsList = 5,
        profileID = 6,
        subgroupsList = 7,
        groupID = 8
    }
}

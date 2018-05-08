using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common
{
    public enum RequestCommand:byte
    {
        Authorization = 1,
        GetPartOfProfile = 2,
        GetGroupList = 3,
        GetColorList = 4,
        GetSubgroupList = 5,
        GetUserList = 6,
        GetTagList = 7
    }
}

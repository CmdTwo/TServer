using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common
{
    public enum ResponseCommand:byte
    {
        AuthorizationResponse = 1,
        GetPartOfProfileResponse = 2,
        GetGroupListResponse = 3,
        GetColorListResponse = 4,
        GetSubgroupListResponse = 5,
        GetUserListResponse = 6,
        GetTagListResponse = 7
    }
}

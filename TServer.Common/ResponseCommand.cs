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
        GetAvailableAccessListResponse = 3,
        GetAvailableSubgroupListResponse = 4,
        GetUserListResponse = 5,
        GetTagListResponse = 6,
        AddNewTestResponse = 7,
        GetAvailableTestsResponse = 8,
        GetFailedTestsResponse = 9,
        GetComplitedTestsResponse = 10,
        GetTestResponse = 11,
        SaveProgressResponse = 12,
        UserCompletedTestResponse = 13

    }
}

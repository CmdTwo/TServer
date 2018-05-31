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
        availableAccessList = 5,
        profileID = 6,
        availableSubAccessList = 7,
        groupID = 8,
        usersList = 9,
        tagList = 10,
        newTest = 11,
        responseStatus = 12,
        availableTests = 13,
        failedTests = 14,
        complitedTests = 15,
        test = 16,
        testID = 17
    }
}

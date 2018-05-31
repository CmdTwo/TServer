﻿using System;
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
        GetAvailableAccessList = 3,
        GetAvailableSubgroupList = 4,
        GetUserList = 5,
        GetTagList = 6,
        AddNewTest = 7,
        GetAvailableTests = 8,
        GetFailedTests = 9,
        GetComplitedTests = 10,
        GetTest = 11,
        UserCompliteTest
    }
}

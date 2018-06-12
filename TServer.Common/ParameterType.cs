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
        testID = 17,
        progress_score = 18,
        progress_skip = 19,
        isCompleted = 20,
        testIsCompleted = 21,
        testHasUsedCount = 22,
        testResultValue = 23,
        questionCount = 24,
        testTime = 25,
        evaluationType = 26,
        previewTestInfo = 27,
        previewTestInfoList = 28,
        positionInTopTestCompleted = 29,
        newAccess = 30,
        otherAccessIDList = 31,
        newTagName = 32,
        progress_time = 33,
        testHaveSaveProgress = 34,
        statisticList = 35,
        testIDList = 36
    }
}

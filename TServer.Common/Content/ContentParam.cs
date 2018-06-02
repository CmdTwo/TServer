using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    public enum ContentParam:byte
    {
        ID = 1,
        name = 2,
        faIcon = 3,
        accessList = 4,
        tagList = 5,
        time = 6,
        canContinueAfterAbort = 7,
        evaluation = 8,
        showAnswerMode = 9,
        questionPageList = 10,
        header = 11,
        questionText = 12,
        answerList = 13,
        questionPageTemplate = 14

    }
}

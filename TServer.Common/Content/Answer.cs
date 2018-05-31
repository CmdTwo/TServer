using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class Answer
    {
        public int ID { get; }
        public string AnswerText { get; }
        public bool IsRightAnswer { get; }

        public Answer(int id, string answerText, bool isRightAnswer)
        {
            ID = id;
            AnswerText = answerText;
            IsRightAnswer = isRightAnswer;
        }

        public Answer(string answerText, bool isRightAnswer)
        {
            AnswerText = answerText;
            IsRightAnswer = isRightAnswer;
        }
    }
}

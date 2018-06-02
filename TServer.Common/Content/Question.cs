using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class Question
    {
        public int ID { get; }
        public string Header { get; private set; }
        public string QuestionText { get; private set; }
        public List<Answer> AnswerList { get; private set; }
        public int QuestionPageTemplate;

        public Question()
        {
            AnswerList = new List<Answer>();
        }

        public Question(int id, string header, string question, int questionPageTemplate)
        {
            ID = id;
            Header = header;
            QuestionText = question;
            AnswerList = new List<Answer>();
            QuestionPageTemplate = questionPageTemplate;
        }

        public Question(string header, string question, List<Answer> answerList)
        {
            Header = header;
            QuestionText = question;
            AnswerList = answerList;
        }

        public void AddToAnswerList(Answer answer)
        {
            AnswerList.Add(answer);
        }

        public void UpdateField(ContentParam contentParam, object obj)
        {
            switch (contentParam)
            {
                case (ContentParam.header):
                    Header = (string)obj;
                    break;
                case (ContentParam.questionText):
                    QuestionText = (string)obj;
                    break;
                case (ContentParam.answerList):
                    AnswerList = (List<Answer>)obj;
                    break;
                case (ContentParam.questionPageTemplate):
                    QuestionPageTemplate = (int)obj;
                    break;
            }
        }

    }
}

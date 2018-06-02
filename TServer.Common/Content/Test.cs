using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class Test
    {
        public int ID { get; }
        public string Name { get; private set; }
        public Profile Profile { get; }
        public List<Access> AccessList { get; private set; }
        public List<Tag> TagList { get; private set; }
        public DateTime Time { get; private set; }
        public bool CanContinueAfterAbort { get; private set; }
        public Evaluation Evaluation { get; private set; }
        public int ShowAnswerMode { get; private set; }
        public List<Question> QuestionPageList { get; private set; }

        public Test()
        {

        }

        public Test(int id, string name, Profile profile, List<Access> accesses, List<Tag> tags, DateTime time, bool canContinue, Evaluation evaluation, int showAnswerMode, List<Question> questionPages)
        {
            ID = id;
            Name = name;
            Profile = profile;
            AccessList = accesses;
            TagList = tags;
            Time = time;
            CanContinueAfterAbort = canContinue;
            Evaluation = evaluation;
            ShowAnswerMode = showAnswerMode;
            QuestionPageList = questionPages;
        }

        public Test(string name, Profile profile, List<Access> accesses, List<Tag> tags, DateTime time, bool canContinue, Evaluation evaluation, int showAnswerMode, List<Question> questionPages)
        {
            Name = name;
            Profile = profile;
            AccessList = accesses;
            TagList = tags;
            Time = time;
            CanContinueAfterAbort = canContinue;
            Evaluation = evaluation;
            ShowAnswerMode = showAnswerMode;
            QuestionPageList = questionPages;
        }

        public void RemoveQuestion(Question question)
        {
            QuestionPageList.Remove(question);
        }

        public void UpdateField(ContentParam contentParam, object obj)
        {
            switch (contentParam)
            {
                case (ContentParam.name):
                    Name = (string)obj;
                    break;
                case (ContentParam.accessList):
                   AccessList = (List<Access>)obj;
                    break;
                case (ContentParam.tagList):
                    TagList = (List<Tag>)obj;
                    break;
                case (ContentParam.time):
                    Time = (DateTime)obj;
                    break;
                case (ContentParam.canContinueAfterAbort):
                    CanContinueAfterAbort = (bool)obj;
                    break;
                case (ContentParam.evaluation):
                    Evaluation = (Evaluation)obj;
                    break;
                case (ContentParam.showAnswerMode):
                    ShowAnswerMode = (int)obj;
                    break;
                case (ContentParam.questionPageList):
                    QuestionPageList = (List<Question>)obj;
                    break;

            }
        }
    }
}

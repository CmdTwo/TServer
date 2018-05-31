using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public abstract class Evaluation
    {
        public Dictionary<int, string> EvaluationDictionary { get; protected set; }
        public List<int> FailedEvaluationValues { get; private set; }

        public Evaluation()
        {
            EvaluationDictionary = new Dictionary<int, string>();
            FailedEvaluationValues = new List<int>();
        }

        public Evaluation(Dictionary<int, string> evaluation)
        {
            EvaluationDictionary = evaluation;
            FailedEvaluationValues = new List<int>();
        }

        public void AddFailedEvaluationValue(int value)
        {
            FailedEvaluationValues.Add(value);
        }
        public void AddEvaluationElement(int key, string value)
        {
            EvaluationDictionary.Add(key, value);
        }
    }

    [Serializable]
    public class Points : Evaluation
    {
        public int MaxPoints { get; private set; }

        public Points(int maxPoints) : base()
        {
            MaxPoints = maxPoints;
        }

        public Points(Dictionary<int, string> evaluation, int maxPoints) : base(evaluation)
        {
            MaxPoints = maxPoints;
        }

        public void UpdateMaxPoints(int value)
        {
            MaxPoints = value;
        }
    }

    [Serializable]
    public class Percent : Evaluation
    {
        public Percent() : base()
        {

        }

        public Percent(Dictionary<int, string> evaluation) : base(evaluation)
        {

        }
    }
}

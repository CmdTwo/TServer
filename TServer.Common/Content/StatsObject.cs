using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TServer.Common.Content
{
    [Serializable]
    public class StatsObject
    {
        private bool IsPercent;

        public int TestID { get; private set; }
        public string TestName { get; private set; }
        public int UserID { get; private set; }
        public string UserName { get; private set; }
        public bool Status { get; private set; }
        public DateTime Date { get; private set; }
        public TimeSpan Time { get; private set; }
        public int Value { get; private set; }
        public List<AccessStats> AccessList { get; private set; }

        [Serializable]
        public struct AccessStats
        {
            public int ID;
            public string Name;
            public Dictionary<int, string> SubAccess;

            public AccessStats(int id, string name, Dictionary<int, string> subAccess)
            {
                ID = id;
                Name = name;
                SubAccess = subAccess;
            }
        }

        public StatsObject(int testID, string testName, int userID, string userName, DateTime date, bool status, TimeSpan time, int value, bool isPercent, List<AccessStats> accessList)
        {
            TestID = testID;
            TestName = testName;
            UserID = userID;
            UserName = userName;
            Status = status;
            Date = date;
            Time = time;
            Value = value;
            IsPercent = isPercent;
            AccessList = accessList;
        }

        public string GetStrValue { get { return (IsPercent == true ? Value + "%" : Value.ToString()); } }
    }
}

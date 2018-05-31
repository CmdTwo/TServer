using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using TServer.Common.Content;

namespace TServer.Generic
{
    public class DBManager
    {
        public object[] Authorized(string login, string password)
        {
            object[] result = null;
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.ID, t_Profile.Name FROM t_Profile WHERE t_Profile.Login = '" + login + "' AND t_Profile.Password = '" + password + "'";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        result = new object[] { true, reader.GetValue(0) };
                    }
                    if(result == null)
                        result = new object[] { false, null };
                }
            }
            return result;
        }

        public object[] GetPartOfProfile(int profileID)
        {
            object[] result = null;
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.Name FROM t_Profile WHERE t_Profile.ID = '" + profileID + "'";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result = new object[] { reader.GetValue(0) };
                    }
                }
            }
            return result;
        }

        public List<Access> GetAvailableAccessList(int profileID)
        {
            List<Access> access = new List<Access>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_AccessGroup.ID, t_AccessGroup.Name, t_AccessGroup.FaIcon FROM t_AccessToGroup INNER JOIN t_AccessGroup ON t_AccessToGroup.IDOtherAccessGroup = t_AccessGroup.ID WHERE t_AccessToGroup.IDAccessGroup = (SELECT distinct t_AccessGroup.ID FROM t_Profile INNER JOIN t_SubAccessGroup ON t_Profile.IDSubAccess = t_SubAccessGroup.ID INNER JOIN t_AccessAndSubGroup ON t_SubAccessGroup.ID = t_AccessAndSubGroup.IDSubAccessGroup INNER JOIN t_AccessGroup ON t_AccessAndSubGroup.IDAccessGroup = t_AccessGroup.ID WHERE t_Profile.ID = " + profileID + ");";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        access.Add(new Access(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1), (string)reader.GetValue(2)));
                    }

                }
            }
            return access;
        }               
        
        public List<SubAccess> GetAvailableSubgroupList(int groupID)
        {
            List<SubAccess> subGroups = new List<SubAccess>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_SubAccessGroup.ID, t_SubAccessGroup.Name FROM t_AccessAndSubGroup INNER JOIN t_SubAccessGroup ON t_AccessAndSubGroup.IDSubAccessGroup = t_SubAccessGroup.ID WHERE t_AccessAndSubGroup.IDAccessGroup = " + groupID;

                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        subGroups.Add(new SubAccess(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1)));
                    }
                }
            }
            return subGroups;
        }

        public Dictionary<int, string> GetUsersList(int profileID)
        {
            Dictionary<int, string> users = new Dictionary<int, string>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.ID, t_Profile.Name FROM t_Profile WHERE t_Profile.IDAccessGroup IN (SELECT t_AccessGroup.ID FROM t_AccessToGroup INNER JOIN t_AccessGroup ON t_AccessToGroup.IDOtherAccessGroup = t_AccessGroup.ID WHERE t_AccessToGroup.IDAccessGroup = (SELECT t_Profile.IDAccessGroup FROM t_Profile WHERE t_Profile.ID = " + profileID + "))";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                }
            }
            return users;
        }

        public List<Tag> GetTagList()
        {
            List<Tag> tags = new List<Tag>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT ID, Name FROM t_Tag";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tags.Add(new Tag(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1)));
                    }
                }
            }
            return tags;
        }

        public bool AddNewTest(Test test, int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "INSERT INTO t_Test(Name, MaxValue, CanContinueAfterAbort, Time, IDEvaluationType, IDProfile, IDShowAnswerType) VALUES ('" + test.Name + "', " + (test.Evaluation is Percent ? "null" : (test.Evaluation as Points).MaxPoints.ToString()) + ", '" + (test.CanContinueAfterAbort ? "y" : "n") + "', " + (test.Time == null ? "0" : ((test.Time.Hour * 60) + test.Time.Minute).ToString()) + ", " + (test.Evaluation is Percent ? 1 : 2) + ", " + profileID + "," + test.ShowAnswerMode + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    int? testID = null;
                    while (reader.Read())
                        testID = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();

                    foreach(Access access in test.AccessList)
                    {
                        if (access.SubAccesses.Count == 0)
                        {
                            cmd.CommandText = "SELECT t_AccessAndSubGroup.IDSubAccessGroup FROM t_AccessAndSubGroup WHERE t_AccessAndSubGroup.IDAccessGroup = " + access.ID;
                            reader = cmd.ExecuteReader();
                            List<int> subAccessIdList = new List<int>();
                            while (reader.Read())
                                subAccessIdList.Add(Convert.ToInt32(reader.GetValue(0)));
                            reader.Close();

                            foreach (int subAccessID in subAccessIdList)
                            {
                                cmd.CommandText = "INSERT INTO t_TestAndAccess(IDTest, IDAccess, IDSubAccess) VALUES (" + testID + ", " + access.ID + ", " + subAccessID + ");";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            foreach (SubAccess subAccess in access.SubAccesses)
                            {
                                cmd.CommandText = "INSERT INTO t_TestAndAccess(IDTest, IDAccess, IDSubAccess) VALUES (" + testID + ", " + access.ID + ", " + subAccess.ID + ");";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    foreach(Tag tag in test.TagList)
                    {
                        cmd.CommandText = "INSERT INTO t_TestAndTag(IDTest, IDTag) VALUES(" + testID + ", " + tag.ID + ");";
                        cmd.ExecuteNonQuery();
                    }

                    foreach(KeyValuePair<int, string> evaluation in test.Evaluation.EvaluationDictionary)
                    {
                        char isFailedValue = 'n';
                        if(test.Evaluation.FailedEvaluationValues.Contains(evaluation.Key))
                        {
                            isFailedValue = 'y';
                        }
                        cmd.CommandText = "INSERT INTO t_Evaluation(Value, TextValue, IsFailedValue) VALUES('" + evaluation.Key + "', '" + evaluation.Value + "', '" + isFailedValue + "');";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        reader = cmd.ExecuteReader();
                        int? evaluationID = null;
                        while (reader.Read())
                            evaluationID = Convert.ToInt32(reader.GetValue(0));
                        reader.Close();

                        cmd.CommandText = "INSERT INTO t_TestAndEvaluation(IDTest, IDEvaluation) VALUES (" + testID + "," + evaluationID + ");";
                        cmd.ExecuteNonQuery();
                    }

                    int questionIndex = 0;

                    foreach(Question question in test.QuestionPageList)
                    {
                        cmd.CommandText = "INSERT INTO t_QuestionPage(Header, Text, IDQuestionTemplate) VALUES ('" + question.Header + "', '" + question.QuestionText + "', " + question.QuestionPageTemplate + ");";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT last_insert_rowid()";
                        reader = cmd.ExecuteReader();
                        int? questionPageID = null;
                        while (reader.Read())
                            questionPageID = Convert.ToInt32(reader.GetValue(0));
                        reader.Close();

                        foreach (Answer answer in question.AnswerList)
                        {
                            cmd.CommandText = "INSERT INTO t_Answer(Value, IsRightAnswer) VALUES ('" + answer.AnswerText + "', '" + (answer.IsRightAnswer ? "y" : "n") + "');";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "SELECT last_insert_rowid()";
                            reader = cmd.ExecuteReader();
                            int? answerID = null;
                            while (reader.Read())
                                answerID = Convert.ToInt32(reader.GetValue(0));
                            reader.Close();

                            cmd.CommandText = "INSERT INTO t_QuestionPageAndAnswer(IDQuestionPage, IDAnswer) VALUES (" + questionPageID + "," + answerID + ");";
                            cmd.ExecuteNonQuery();
                        }
                        cmd.CommandText = "INSERT INTO t_TestAndQuestionPage(IDTest, IDQuestionPage, NumIndex) VALUES (" + testID + ", " + questionPageID + ", " + questionIndex++ + ");";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public List<object[]> GetAvailableTests(int profileID)
        {
            List<int> testIDs = GetAvailableTestsID(profileID);
            List<object[]> testPreviews = new List<object[]>();
            if (testIDs.Count != 0)
            {
                using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        con.Open();
                        cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT (t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time,  t_Test.IDEvaluationType FROM t_Test WHERE t_Test.ID IN(" + string.Join(",", testIDs) + ");";
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            testPreviews.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4) });
                        }

                    }
                }
            }
            else
                return new List<object[]>();
            return testPreviews;
        }

        public List<object[]> GetFailedTests(int profileID)
        {
            List<object[]> testPreviews = new List<object[]>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType FROM t_Test INNER JOIN t_Result ON t_Test.ID = t_Result.IDTest WHERE t_Result.IDStatus = 2 and t_Result.IDProfile = " + profileID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testPreviews.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4) });
                    }
                }
            }
            return testPreviews;
        }

        public List<object[]> GetComplitedTests(int profileID)
        {
            List<object[]> testPreviews = new List<object[]>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType FROM t_Test INNER JOIN t_Result ON t_Test.ID = t_Result.IDTest WHERE t_Result.IDStatus = 1 and t_Result.IDProfile = " + profileID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testPreviews.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4) });
                    }
                }
            }
            return testPreviews;
        }

        public List<int> GetAvailableTestsID(int profileID)
        {
            List<int> testIds = new List<int>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT distinct t_Test.ID FROM t_Test INNER JOIN t_TestAndAccess ON t_Test.ID = t_TestAndAccess.IDTest WHERE t_TestAndAccess.IDSubAccess IN (SELECT distinct t_Profile.IDSubAccess FROM t_Profile WHERE t_Profile.ID = " + profileID + ")";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testIds.Add(Convert.ToInt32(reader.GetValue(0)));
                    }
                }
            }
            return testIds;
        }

        public Test GetTest(int testID)
        {
            Test test = null;
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT * FROM t_Test WHERE t_Test.ID = " + testID;
                    List<object> returnData_Test = new List<object>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        returnData_Test.Add(reader.GetValue(0));
                        returnData_Test.Add(reader.GetValue(1));
                        returnData_Test.Add(reader.GetValue(2));
                        returnData_Test.Add(reader.GetValue(3));
                        returnData_Test.Add(reader.GetValue(4));
                        returnData_Test.Add(reader.GetValue(5));
                        returnData_Test.Add(reader.GetValue(6));
                        returnData_Test.Add(reader.GetValue(7));
                    }
                    reader.Close();

                    cmd.CommandText = "SELECT t_Profile.ID, t_Profile.Name FROM t_Profile WHERE t_Profile.ID = " + returnData_Test[6];
                    Profile profile = null;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        profile = new Profile(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString());
                    reader.Close();

                    cmd.CommandText = "SELECT * FROM t_AccessGroup WHERE t_AccessGroup.ID IN (SELECT distinct IDAccess FROM t_TestAndAccess WHERE t_TestAndAccess.IDTest = " + testID + ")";
                    List<Access> accessList = new List<Access>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        accessList.Add(new Access(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString(), reader.GetValue(2).ToString()));
                    reader.Close();

                    foreach (Access access in accessList)
                    {
                        cmd.CommandText = "SELECT * FROM t_SubAccessGroup WHERE t_SubAccessGroup.ID IN (SELECT t_TestAndAccess.IDSubAccess FROM t_TestAndAccess WHERE t_TestAndAccess.IDAccess = " + access.ID + " AND t_TestAndAccess.IDTest = " + testID + ")";
                        List<SubAccess> allSubAccess = new List<SubAccess>();
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            allSubAccess.Add(new SubAccess(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString()));
                        reader.Close();
                        access.SubAccesses = allSubAccess;
                    }

                    cmd.CommandText = "SELECT * FROM t_Tag WHERE t_Tag.ID IN(select t_TestAndTag.IDTag FROM t_TestAndTag WHERE t_TestAndTag.IDTest = " + testID + ")";
                    List<Tag> tagList = new List<Tag>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        tagList.Add(new Tag(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString()));
                    reader.Close();

                    cmd.CommandText = "SELECT * FROM t_EvaluationType WHERE t_EvaluationType.ID = " + returnData_Test[5].ToString();
                    reader = cmd.ExecuteReader();
                    Evaluation evaluation = null;
                    while (reader.Read())
                        if (Convert.ToUInt32(reader.GetValue(0)) == 1)
                            evaluation = new Percent();
                        else
                            evaluation = new Points(Convert.ToInt32(returnData_Test[2]));
                    reader.Close();

                    cmd.CommandText = "SELECT t_Evaluation.Value, t_Evaluation.TextValue, t_Evaluation.IsFailedValue FROM t_Evaluation WHERE t_Evaluation.ID IN(SELECT t_TestAndEvaluation.IDEvaluation FROM t_TestAndEvaluation WHERE t_TestAndEvaluation.IDTest = " + testID + ")";
                    Dictionary<int, string> evaluationDic = new Dictionary<int, string>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        evaluation.AddEvaluationElement(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                        if(reader.GetValue(2).ToString() == "y")
                        {
                            evaluation.AddFailedEvaluationValue(Convert.ToInt32(reader.GetValue(0)));
                        }

                    }
                    reader.Close();
                    bool canContinueAfterAbort = (returnData_Test[4].ToString() == "y" ? true : false);

                    cmd.CommandText = "SELECT t_ShowAnswerType.ID FROM t_ShowAnswerType WHERE t_ShowAnswerType.ID = " + returnData_Test[7].ToString();
                    reader = cmd.ExecuteReader();
                    int showAnswerType = 0;
                    while (reader.Read())
                        showAnswerType = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();                        

                    cmd.CommandText = "SELECT * FROM t_QuestionPage WHERE t_QuestionPage.ID IN (SELECT t_TestAndQuestionPage.IDQuestionPage FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest =" + testID + ")";
                    List<Question> questionsList = new List<Question>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        questionsList.Add(new Question(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), Convert.ToInt32(reader.GetValue(3))));
                    reader.Close();

                    foreach (Question question in questionsList)
                    {
                        cmd.CommandText = "SELECT t_Answer.Value, t_Answer.IsRightAnswer FROM t_Answer WHERE t_Answer.ID IN (SELECT t_QuestionPageAndAnswer.IDAnswer FROM t_QuestionPageAndAnswer WHERE t_QuestionPageAndAnswer.IDQuestionPage =" + question.ID + ")";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            question.AddToAnswerList(new Answer(reader.GetValue(0).ToString(), (reader.GetValue(1).ToString() == "y" ? true : false)));
                        reader.Close();
                    }
                    test = new Test(Convert.ToInt32(returnData_Test[0]), returnData_Test[1].ToString(), profile, accessList, tagList, new DateTime().AddMinutes(Convert.ToDouble(returnData_Test[4])), canContinueAfterAbort, evaluation, showAnswerType, questionsList);
                }
            }
            return test;
        }
    }
}

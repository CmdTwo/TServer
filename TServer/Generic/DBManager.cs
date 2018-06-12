using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using TServer.Common.Content;
using TServer.Common;

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
                    cmd.CommandText = "INSERT INTO t_Test(Name, MaxValue, CanContinueAfterAbort, Time, IDEvaluationType, IDProfile, IDShowAnswerType) VALUES ('" + test.Name + "', " + (test.Evaluation is Percent ? "null" : (test.Evaluation as Points).MaxPoints.ToString()) + ", '" + (test.CanContinueAfterAbort ? "y" : "n") + "', '" + test.Time.TotalMinutes + "', " + (test.Evaluation is Percent ? 1 : 2) + ", " + profileID + "," + test.ShowAnswerMode + ")";
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

        public List<Dictionary<ParameterType, object>> GetAvailableTests(int profileID)
        {
            List<int> testIDs = GetAvailableTestsID(profileID);
            List<Dictionary<ParameterType, object>> testPreviewsList = new List<Dictionary<ParameterType, object>>();
            if (testIDs.Count != 0)
            {
                using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        con.Open();

                        cmd.CommandText = "SELECT distinct t_TestCollect.IDTest FROM t_TestCollect WHERE t_TestCollect.IDTest IN (" + string.Join(", ", testIDs) + ")";
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        List<int> collectTestIDs = new List<int>();
                        while (reader.Read())
                        {
                            collectTestIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                            break;
                        }
                        reader.Close();

                        if(collectTestIDs.Count != 0)
                        {
                            cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestCollect.ID) FROM t_TestCollect WHERE t_TestCollect.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType FROM t_Test WHERE t_Test.ID IN(" + string.Join(",", collectTestIDs) + ")";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                                //testPreviews.Add(new List<object> { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5)});
                                testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                                testPreview.Add(ParameterType.name, reader.GetString(1));
                                testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                                testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                                testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                                //testPreview.Add(ParameterType.testHistoryStatus, reader.GetValue(5));
                                testPreviewsList.Add(testPreview);
                            }
                            reader.Close();
                        }

                         cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType FROM t_Test WHERE t_Test.ID IN(" + string.Join(",", testIDs.Except(collectTestIDs)) + ")";

                        //cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, (SELECT CASE WHEN t_Result.IDTestingHistory IS null THEN 'n' ELSE 'y' END AS CHAR FROM t_Result WHERE t_Result.IDTest = t_Test.ID) FROM t_Test INNER JOIN t_Result ON t_Test.ID = t_Result.IDTest WHERE t_Test.ID IN(2,3,4,5)";
                        //cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, , t_Test.Time, t_Test.IDEvaluationType, (SELECT CASE WHEN t_Result.IDTestingHistory IS null THEN 'n' ELSE 'y' END AS CHAR FROM t_Result WHERE t_Result.IDTest = t_Test.ID) FROM t_Test INNER JOIN t_Result ON t_Test.ID = t_Result.IDTest WHERE t_Test.ID IN(" + string.Join(",", testIDs) + ") AND t_Result.IDProfile = " + profileID;
                        
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                            //testPreviews.Add(new List<object> { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5)});
                            testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                            testPreview.Add(ParameterType.name, reader.GetString(1));
                            testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                            testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                            testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                            //testPreview.Add(ParameterType.testHistoryStatus, reader.GetValue(5));
                            testPreviewsList.Add(testPreview);
                        }
                        reader.Close();

                        foreach(Dictionary<ParameterType, object> element in testPreviewsList)
                        {
                            cmd.CommandText = "SELECT t_Tag.ID FROM t_TestAndTag INNER JOIN t_Tag ON t_TestAndTag.IDTag = t_Tag.ID WHERE t_TestAndTag.IDTest = " + element[ParameterType.testID];
                            reader = cmd.ExecuteReader();
                            List<int> tagIDs = new List<int>();
                            while (reader.Read())
                            {
                                tagIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                            }
                            reader.Close();

                            element.Add(ParameterType.tagList, tagIDs);
                            cmd.CommandText = "SELECT t_TestingHistory.IsCompleted FROM t_TestingHistory WHERE t_TestingHistory.IDTest = " + element[ParameterType.testID] + " AND t_TestingHistory.IDProfile = " + profileID + " ORDER BY Date DESC LIMIT 1";
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                element.Add(ParameterType.isCompleted, reader.GetValue(0));
                            }
                            reader.Close();                            
                        }
                    }
                }
            }
            else
                return new List<Dictionary<ParameterType, object>>();
            return testPreviewsList;
        }

        public List<Dictionary<ParameterType, object>> GetFailedTests(int profileID)
        {
            List<Dictionary<ParameterType, object>> testPreviewsList = new List<Dictionary<ParameterType, object>>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();

                    cmd.CommandText = "SELECT t_TestCollect.IDTest FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID INNER JOIN t_TestCollect ON t_Result.IDTest = t_TestCollect.IDTest WHERE t_TestingHistory.IsCompleted = 'n' AND t_Result.IDProfile =" + profileID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    List<int> collectTestIDs = new List<int>();
                    while (reader.Read())
                    {
                        collectTestIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                        break;
                    }
                    reader.Close();

                    if (collectTestIDs.Count != 0)
                    {
                        cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestCollect.ID) FROM t_TestCollect WHERE t_TestCollect.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType,  t_TestingHistory.IsCompleted FROM t_Test WHERE t_Test.ID IN(" + string.Join(",", collectTestIDs) + ")";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                            testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                            testPreview.Add(ParameterType.name, reader.GetString(1));
                            testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                            testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                            testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                            testPreview.Add(ParameterType.isCompleted, reader.GetValue(5));
                            testPreviewsList.Add(testPreview);
                        }
                        reader.Close();
                    }

                    //cmd.CommandText = "SELECT distinct t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_Test INNER JOIN t_Result ON t_Test.ID = t_Result.IDTest INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID WHERE t_TestingHistory.IsCompleted = 'n' AND t_Result.IDProfile = " + profileID;
                    //cmd.CommandText = "SELECT distinct t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_TestingHistory INNER JOIN t_Test ON t_Test.ID = t_TestingHistory.IDTest WHERE t_TestingHistory.IDProfile = "+ profileID + " AND t_TestingHistory.IsCompleted = 'n' ORDER BY Date DESC";
                    //cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_TestingHistory INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.ID IN (SELECT distinct (SELECT distinct t_TestingHistory.ID FROM t_TestingHistory WHERE t_TestingHistory.IDProfile = " + profileID + " AND t_TestingHistory.IDTest = t_Test.ID ORDER BY t_TestingHistory.Date DESC LIMIT 1) FROM t_TestingHistory INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.IDProfile = " + profileID + ")";
                    cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.IsCompleted = 'n' AND t_Result.IDProfile =" + profileID;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if(!collectTestIDs.Contains(Convert.ToInt32(reader.GetValue(0))))
                        {
                            Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                            testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                            testPreview.Add(ParameterType.name, reader.GetString(1));
                            testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                            testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                            testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                            testPreview.Add(ParameterType.isCompleted, reader.GetValue(5));
                            testPreviewsList.Add(testPreview);
                        }
                        
                    }
                    reader.Close();

                    foreach (Dictionary<ParameterType, object> element in testPreviewsList)
                    {
                        cmd.CommandText = "SELECT t_Tag.ID FROM t_TestAndTag INNER JOIN t_Tag ON t_TestAndTag.IDTag = t_Tag.ID WHERE t_TestAndTag.IDTest = " + element[ParameterType.testID];
                        reader = cmd.ExecuteReader();
                        List<int> tagIDs = new List<int>();
                        while (reader.Read())
                        {
                            tagIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                        }
                        reader.Close();
                        element.Add(ParameterType.tagList, tagIDs);
                    }
                }
            }
            return testPreviewsList;
        }

        public List<Dictionary<ParameterType, object>> GetComplitedTests(int profileID)
        {
            List<Dictionary<ParameterType, object>> testPreviewsList = new List<Dictionary<ParameterType, object>>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();

                    cmd.CommandText = "SELECT t_TestCollect.IDTest FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID INNER JOIN t_TestCollect ON t_Result.IDTest = t_TestCollect.IDTest WHERE t_TestingHistory.IsCompleted = 'y' AND t_Result.IDProfile =" + profileID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    List<int> collectTestIDs = new List<int>();
                    while (reader.Read())
                    {
                        collectTestIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                        break;
                    }
                    reader.Close();

                    if (collectTestIDs.Count != 0)
                    {
                        cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestCollect.ID) FROM t_TestCollect WHERE t_TestCollect.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted  FROM t_Test WHERE t_Test.ID IN(" + string.Join(",", collectTestIDs) + ")";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                            testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                            testPreview.Add(ParameterType.name, reader.GetString(1));
                            testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                            testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                            testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                            testPreview.Add(ParameterType.isCompleted, reader.GetValue(5));
                            testPreviewsList.Add(testPreview);
                        }
                        reader.Close();
                    }

                    //cmd.CommandText = "SELECT distinct t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_Test INNER JOIN t_TestingHistory ON t_Test.ID = t_TestingHistory.IDTest WHERE t_TestingHistory.IsCompleted = 'y' AND t_TestingHistory.IDProfile =" + profileID;
                    //cmd.CommandText = "SELECT distinct t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_TestingHistory INNER JOIN t_Test ON t_Test.ID = t_TestingHistory.IDTest WHERE t_TestingHistory.IDProfile = " + profileID + " AND t_TestingHistory.IsCompleted = 'y' ORDER BY Date DESC";
                    //SQLiteDataReader reader = cmd.ExecuteReader();
                    //cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_TestingHistory INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.ID IN (SELECT distinct (SELECT distinct t_TestingHistory.ID FROM t_TestingHistory WHERE t_TestingHistory.IDProfile = " + profileID + " AND t_TestingHistory.IDTest = t_Test.ID ORDER BY t_TestingHistory.Date DESC LIMIT 1) FROM t_TestingHistory INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.IDProfile = " + profileID + ")";
                    cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, (SELECT COUNT(t_TestAndQuestionPage.ID) FROM t_TestAndQuestionPage WHERE t_TestAndQuestionPage.IDTest = t_Test.ID), t_Test.Time, t_Test.IDEvaluationType, t_TestingHistory.IsCompleted FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID WHERE t_TestingHistory.IsCompleted = 'y' AND t_Result.IDProfile =" + profileID;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (!collectTestIDs.Contains(Convert.ToInt32(reader.GetValue(0))))
                        {
                            Dictionary<ParameterType, object> testPreview = new Dictionary<ParameterType, object>();
                            testPreview.Add(ParameterType.testID, Convert.ToInt32(reader.GetValue(0)));
                            testPreview.Add(ParameterType.name, reader.GetString(1));
                            testPreview.Add(ParameterType.questionCount, Convert.ToInt32(reader.GetValue(2)));
                            testPreview.Add(ParameterType.testTime, reader.GetValue(3));
                            testPreview.Add(ParameterType.evaluationType, reader.GetValue(4));
                            testPreview.Add(ParameterType.isCompleted, reader.GetValue(5));
                            testPreviewsList.Add(testPreview);
                        }

                    }
                    reader.Close();
                    foreach (Dictionary<ParameterType, object> element in testPreviewsList)
                    {
                        cmd.CommandText = "SELECT t_Tag.ID FROM t_TestAndTag INNER JOIN t_Tag ON t_TestAndTag.IDTag = t_Tag.ID WHERE t_TestAndTag.IDTest = " + element[ParameterType.testID];
                        reader = cmd.ExecuteReader();
                        List<int> tagIDs = new List<int>();
                        while (reader.Read())
                        {
                            tagIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                        }
                        reader.Close();
                        element.Add(ParameterType.tagList, tagIDs);
                    }
                }
            }
            return testPreviewsList;
        }

        private List<int> GetAvailableTestsID(int profileID)
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
                    bool canContinueAfterAbort = (returnData_Test[3].ToString() == "y" ? true : false);

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
                    test = new Test(Convert.ToInt32(returnData_Test[0]), returnData_Test[1].ToString(), profile, accessList, tagList, Convert.ToInt32(returnData_Test[4]), canContinueAfterAbort, evaluation, showAnswerType, questionsList);
                }
            }
            return test;
        }

        public List<object> GetTestAndStats(int testID, int profileID)
        {
            Test test = GetTest(testID);
            List<object> testAndInfo = new List<object>() { test };

            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();

                    cmd.CommandText = "SELECT t_SaveProgress.ID FROM t_SaveProgress WHERE t_SaveProgress.IDProfile = " + profileID + " AND t_SaveProgress.IDTest =" + testID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        testAndInfo.Add(true);
                    }
                    if(testAndInfo.Count == 1)
                        testAndInfo.Add(false);

                    reader.Close();

                    //cmd.CommandText = "SELECT t_TestingHistory.IsCompleted, t_TestingHistory.Value, (SELECT COUNT(t_TestingHistory.ID) FROM t_TestingHistory WHERE t_TestingHistory.IDTest = t_Result.IDTest) FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID WHERE t_Result.IDTest =" + testID;
                    //cmd.CommandText = "SELECT t_TestingHistory.IsCompleted, t_TestingHistory.Value FROM t_TestingHistory WHERE t_TestingHistory.IDTest = " + testID + " AND t_TestingHistory.IDProfile = " + profileID + " ORDER BY Date DESC LIMIT 1";
                    cmd.CommandText = "SELECT t_TestingHistory.IsCompleted, t_TestingHistory.Value FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID WHERE t_TestingHistory.IDTest = " + testID + " AND t_Result.IDProfile =" + profileID;

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testAndInfo.Add(reader.GetString(0) == "y" ? true : false);
                        testAndInfo.Add(Convert.ToInt32(reader.GetValue(1)));
                    }
                    reader.Close();

                    cmd.CommandText = "SELECT t_Result.IDProfile FROM t_Result INNER JOIN t_TestingHistory ON t_Result.IDTestingHistory = t_TestingHistory.ID WHERE t_TestingHistory.IDTest = " + testID + " ORDER BY t_TestingHistory.Value DESC";
                    reader = cmd.ExecuteReader();
                    int position = 1;
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader.GetValue(0)) == profileID)
                        {
                            testAndInfo.Add(position);
                            break;
                        }
                        else
                        {
                            position++;
                        }
                    }
                    reader.Close();
                }
            }
            return testAndInfo;
        }

        public bool SaveProgress(int testID, double score, int skip, double? time, int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    string timeStr = time == null ? "null" : "'" + time.ToString() + "'";
                    cmd.CommandText = "INSERT INTO t_SaveProgress(IDTest, IDProfile, Score, Time, SkipQuestion) VALUES(" + testID + "," + profileID + "," + score.ToString().Replace(',','.') + "," + timeStr + "," + skip + ")";
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public bool UserCompletedTest(int testID, int score, int profileID, bool isCompleted, string time)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "INSERT INTO t_TestingHistory(IDTest, IDProfile, Date, Time, Value, IsCompleted) VALUES(" + testID + "," + profileID + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + time + "'," + score + "," + (isCompleted ? "'y'" : "'n'") + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                }
            }
            return true;
        }

        public bool AddNewAccess(Access newAccess, List<int> otherAccessIDList)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "INSERT INTO t_AccessGroup(Name, FaIcon) VALUES('" + newAccess.Name + "', '" + newAccess.FaIcon + "')";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    int? accessID = null;
                    while (reader.Read())
                        accessID = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();

                    foreach(SubAccess subAccess in newAccess.SubAccesses)
                    {
                        cmd.CommandText = "INSERT INTO t_SubAccessGroup (Name) VALUES ('" + subAccess.Name + "');";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        reader = cmd.ExecuteReader();
                        int? subAccessID = null;
                        while (reader.Read())
                            subAccessID = Convert.ToInt32(reader.GetValue(0));
                        reader.Close();

                        cmd.CommandText = "INSERT INTO t_AccessAndSubGroup (IDAccessGroup, IDSubAccessGroup) VALUES (" + accessID + "," + subAccessID + ")";
                        cmd.ExecuteNonQuery();
                    }

                    otherAccessIDList.Add((int)accessID);

                    foreach (int otherAccessID in otherAccessIDList)
                    {
                        cmd.CommandText = "INSERT INTO t_AccessToGroup (IDAccessGroup, IDOtherAccessGroup) VALUES (" + otherAccessID + "," + accessID + ")";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public bool AddNewTag(string tagName)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "INSERT INTO t_Tag(Name) VALUES('" + tagName + "')";
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public List<object> ContinueTest(int testID, int profileID)
        {
            List<object> saveProgress = new List<object>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_SaveProgress.Score, t_SaveProgress.SkipQuestion, t_SaveProgress.Time FROM t_SaveProgress WHERE t_SaveProgress.IDProfile = " + profileID + " AND t_SaveProgress.IDTest =" + testID;
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saveProgress.Add(Convert.ToInt32(reader.GetValue(0)));
                        saveProgress.Add(Convert.ToInt32(reader.GetValue(1)));
                        saveProgress.Add(Convert.ToDouble(reader.GetValue(2)));
                    }
                    reader.Close();
                }
            }
            return saveProgress;
        }

        public List<StatsObject> GetStatistic()
        {
            List<StatsObject> statistics = new List<StatsObject>();
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Test.ID, t_Test.Name, t_Profile.ID, t_Profile.Name, t_TestingHistory.Date, t_TestingHistory.Time, t_TestingHistory.IsCompleted, t_TestingHistory.Value, t_Test.IDEvaluationType FROM t_TestingHistory INNER JOIN t_Profile ON t_TestingHistory.IDProfile = t_Profile.ID INNER JOIN t_Test ON t_TestingHistory.IDTest = t_Test.ID";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    List<List<object>> result = new List<List<object>>();
                    while (reader.Read())
                    {
                        result.Add(new List<object>() { Convert.ToInt32(reader.GetValue(0)), reader.GetString(1), Convert.ToInt32(reader.GetValue(2)), reader.GetString(3), reader.GetDateTime(4), TimeSpan.Parse(reader.GetString(5)), reader.GetString(6) == "y" ? true : false, Convert.ToInt32(reader.GetValue(7)), Convert.ToInt32(reader.GetValue(8)) == 1 });
                    }
                    reader.Close();

                    foreach(List<object> element in result)
                    {
                        Dictionary<int, string> accessDictionary = new Dictionary<int, string>();
                        cmd.CommandText = "SELECT t_AccessGroup.ID, t_AccessGroup.Name FROM t_AccessGroup WHERE t_AccessGroup.ID IN (SELECT distinct IDAccess FROM t_TestAndAccess WHERE t_TestAndAccess.IDTest =" + element[0] + ")";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            accessDictionary.Add(Convert.ToInt32(reader.GetValue(0)), reader.GetString(1));    
                        }
                        reader.Close();

                        List<StatsObject.AccessStats> accessStats = new List<StatsObject.AccessStats>();

                        foreach(KeyValuePair<int, string> access in accessDictionary)
                        {
                            cmd.CommandText = "SELECT t_SubAccessGroup.ID, t_SubAccessGroup.Name FROM t_SubAccessGroup WHERE t_SubAccessGroup.ID IN (SELECT t_TestAndAccess.IDSubAccess FROM t_TestAndAccess WHERE t_TestAndAccess.IDAccess = " + access.Key + " AND t_TestAndAccess.IDTest = " + element[0] + ")";
                            reader = cmd.ExecuteReader();
                            Dictionary<int, string> subAccessDictionary = new Dictionary<int, string>();
                            while (reader.Read())
                            {
                                subAccessDictionary.Add(Convert.ToInt32(reader.GetValue(0)), reader.GetString(1));
                            }
                            reader.Close();

                            accessStats.Add(new StatsObject.AccessStats(access.Key, access.Value, subAccessDictionary));
                        }
                        statistics.Add(new StatsObject((int)element[0], (string)element[1], (int)element[2], (string)element[3], (DateTime)element[4], (bool)element[6], (TimeSpan)element[5], Convert.ToInt32(element[7]), (bool)element[8], accessStats));
                    }
                }
            }
            return statistics;
        }

        public bool AddCollectTest(Test test, List<int> testIDList, int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "INSERT INTO t_Test(Name, MaxValue, CanContinueAfterAbort, Time, IDEvaluationType, IDProfile, IDShowAnswerType) VALUES ('" + test.Name + "', " + (test.Evaluation is Percent ? "null" : (test.Evaluation as Points).MaxPoints.ToString()) + ", '" + (test.CanContinueAfterAbort ? "y" : "n") + "', '" + test.Time.TotalMinutes + "', " + (test.Evaluation is Percent ? 1 : 2) + ", " + profileID + "," + test.ShowAnswerMode + ")";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    int? testID = null;
                    while (reader.Read())
                        testID = Convert.ToInt32(reader.GetValue(0));
                    reader.Close();
                   
                    cmd.CommandText = "SELECT distinct t_TestAndAccess.IDAccess FROM t_TestAndAccess WHERE t_TestAndAccess.IDTest IN (" + string.Join(",", testIDList) + ")";
                    reader = cmd.ExecuteReader();
                    List<int> accessIdList = new List<int>();
                    while (reader.Read())
                        accessIdList.Add(Convert.ToInt32(reader.GetValue(0)));
                    reader.Close();

                    foreach (int accessID in accessIdList)
                    {
                        cmd.CommandText = "SELECT t_AccessAndSubGroup.IDSubAccessGroup FROM t_AccessAndSubGroup WHERE t_AccessAndSubGroup.IDAccessGroup = " + accessID;
                        reader = cmd.ExecuteReader();
                        List<int> subAccessIdList = new List<int>();
                        while (reader.Read())
                            subAccessIdList.Add(Convert.ToInt32(reader.GetValue(0)));
                        reader.Close();

                        foreach (int subAccessID in subAccessIdList)
                        {
                            cmd.CommandText = "INSERT INTO t_TestAndAccess(IDTest, IDAccess, IDSubAccess) VALUES (" + testID + ", " + accessID + ", " + subAccessID + ");";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    
                    foreach (KeyValuePair<int, string> evaluation in test.Evaluation.EvaluationDictionary)
                    {
                        char isFailedValue = 'n';
                        if (test.Evaluation.FailedEvaluationValues.Contains(evaluation.Key))
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

                    foreach(int element in testIDList)
                    {
                        cmd.CommandText = "INSERT INTO t_TestCollect(IDTest, IDOtherTest) VALUES (" + testID + "," + element + ");";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        //public List<Dictionary<ParameterType, object>> GetCollect()
        //{

        //}
    }
}

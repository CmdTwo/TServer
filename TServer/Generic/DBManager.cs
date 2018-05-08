using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace TServer.Generic
{
    public class DBManager
    {
        public object[] Authorized(string login, string password)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.ID, t_Profile.Name FROM t_Profile WHERE t_Profile.Login = '" + login + "' AND t_Profile.Password = '" + password + "'";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        return new object[] { true, reader.GetValue(0) };
                    }
                    return new object[] { false, null };
                }
            }
        }

        public object[] GetPartOfProfile(int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.Name FROM t_Profile WHERE t_Profile.ID = '" + profileID + "'";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        return new object[] { reader.GetValue(0) };
                    }
                    return null;
                }
            }            
        }

        public Dictionary<int, string> GetGroupsList(int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_AccessGroup.ID, t_AccessGroup.Name FROM t_AccessToGroup INNER JOIN t_AccessGroup ON t_AccessToGroup.IDOtherAccessGroup = t_AccessGroup.ID WHERE t_AccessToGroup.IDAccessGroup = (SELECT t_Profile.IDAccessGroup FROM t_Profile WHERE t_Profile.ID = " + profileID + ")";
                    Dictionary<int, string> groups = new Dictionary<int, string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        groups.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                    return groups;
                }
            }
        }

        public Dictionary<int, string> GetColorList()
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT * FROM t_Color";
                    Dictionary<int, string> colors = new Dictionary<int, string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        colors.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                    return colors;
                }
            }
        }
        
        public Dictionary<int, string> GetSubgroupList(int groupID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_SubAccessGroup.ID, t_SubAccessGroup.Name FROM t_AccessAndSubGroup INNER JOIN t_SubAccessGroup ON t_AccessAndSubGroup.IDSubAccessGroup = t_SubAccessGroup.ID WHERE t_AccessAndSubGroup.IDAccessGroup = " + groupID;
                    Dictionary<int, string> subGroups = new Dictionary<int, string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        subGroups.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                    return subGroups;
                }
            }
        }

        public Dictionary<int, string> GetUsersList(int profileID)
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT t_Profile.ID, t_Profile.Name FROM t_Profile WHERE t_Profile.IDAccessGroup IN (SELECT t_AccessGroup.ID FROM t_AccessToGroup INNER JOIN t_AccessGroup ON t_AccessToGroup.IDOtherAccessGroup = t_AccessGroup.ID WHERE t_AccessToGroup.IDAccessGroup = (SELECT t_Profile.IDAccessGroup FROM t_Profile WHERE t_Profile.ID = " + profileID + "))";
                    Dictionary<int, string> users = new Dictionary<int, string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                    return users;
                }
            }
        }

        public Dictionary<int, string> GetTagList()
        {
            using (SQLiteConnection con = new SQLiteConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    con.Open();
                    cmd.CommandText = "SELECT ID, Name FROM t_Tag";
                    Dictionary<int, string> tags = new Dictionary<int, string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tags.Add(Convert.ToInt32(reader.GetValue(0)), (string)reader.GetValue(1));
                    }
                    return tags;
                }
            }
        }
    }
}

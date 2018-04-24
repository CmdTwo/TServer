using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace TServer.Generic
{
    public class DBManager
    {
        public object[] Authorized(string login, string password)
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FindProfile", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Login", SqlDbType.NVarChar).Value = login;
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = password;
                    con.Open();                    
                    SqlDataReader reader = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetPartOfProfile", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProfileID", SqlDbType.NVarChar).Value = profileID;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetGroups", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProfileID", SqlDbType.NVarChar).Value = profileID;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> groups = new Dictionary<int, string>();
                    while (reader.Read())
                    {
                        groups.Add((int)reader.GetValue(0), (string)reader.GetValue(1));
                    }
                    return groups;
                }
            }
        }

        public Dictionary<int, string> GetColorList()
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM t_Color", con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> groups = new Dictionary<int, string>();
                    while (reader.Read())
                    {
                        groups.Add((int)reader.GetValue(0), (string)reader.GetValue(1));
                    }
                    return groups;
                }
            }
        }


        public Dictionary<int, string> GetSubgroupList(int groupID)
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.db_TestloConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSubgroups", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@GroupID", SqlDbType.NVarChar).Value = groupID;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> groups = new Dictionary<int, string>();
                    while (reader.Read())
                    {
                        groups.Add((int)reader.GetValue(0), (string)reader.GetValue(1));
                    }
                    return groups;
                }
            }
        }
    }
}

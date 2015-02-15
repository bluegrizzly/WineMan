using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Wine_Type
    {
        public const string c_dbName = "wine_types";
        public int id = 0;
        public string name;
        public bool active;
        public int brand_id;
        public int category_id;

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                int numValue;
                bool parsed = Int32.TryParse(dr["id"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                id = numValue;
                name = dr["name"].ToString();
                name.Replace("\"", "");
                active = dr["active"].ToString() == "1" ? true : false;
                parsed = Int32.TryParse(dr["brand_id"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                brand_id = numValue;
                parsed = Int32.TryParse(dr["category_id"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                category_id = numValue;
            }
        }

        public static string GetDoublonValidationSqlQuery(System.Collections.Specialized.NameValueCollection forms)
        {
            string sqlCmd = "SELECT * FROM " + c_dbName + " WHERE name='" + forms.Get("name").ToString() + "'" +
                " AND brand_id='" + forms.Get("brand_id").ToString() + "'";
            return sqlCmd;
        }

        public static Wine_Type GetRecordByID(string idstr)
        {
            List<Wine_Type> ret = GetRecords("SELECT DISTINCT * FROM " + c_dbName + " WHERE id LIKE '" + idstr + "'");
            if (ret.Count == 1)
                return ret[0];
            else
                return null;
        }
        public static List<Wine_Type> GetAllRecords(bool activeOnly=true)
        {
            string sqlQuery = "SELECT DISTINCT * FROM " + c_dbName;
            if (activeOnly)
                sqlQuery += " WHERE active=1";
            return GetRecords(sqlQuery);
        }
        public static List<Wine_Type> GetAllRecordsWithCategory(string categoryID)
        {
            return GetRecords("SELECT DISTINCT * FROM " + c_dbName + " WHERE category_id='" + categoryID + "'");
        }
        public static List<Wine_Type> GetAllRecordsWithBrand(string brandID)
        {
            return GetRecords("SELECT DISTINCT * FROM " + c_dbName + " WHERE brand_id='" + brandID + "'");
        }

        protected static List<Wine_Type> GetRecords(string sqlQuery)
        {
            List<Wine_Type> ret = new List<Wine_Type>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Wine_Type wineType = new Wine_Type();
                        wineType.FillData(dr);
                        ret.Add(wineType);
                    }
                }
                con.Close();
            }

            return ret;
        }
     }
}
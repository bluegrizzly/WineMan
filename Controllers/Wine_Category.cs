using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Wine_Category
    {
        public int id=-1;
        public string name;
        public float cost;
        public int step;
        public int days;
        public string symbol;

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                int numValue;
                float floatValue;

                bool parsed = Int32.TryParse(dr["id"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                id = numValue;

                name = dr["name"].ToString();
                name = name.Trim('\"');

                parsed = Single.TryParse(dr["cost"].ToString(), out floatValue);
                System.Diagnostics.Debug.Assert(parsed);
                cost = floatValue;

                parsed = Int32.TryParse(dr["step"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                step = numValue;

                parsed = Int32.TryParse(dr["days"].ToString(), out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                days = numValue;

                symbol = dr["symbol"].ToString();
            }
        }

        public static Wine_Category GetRecordByID(string id)
        {
            Wine_Category ret = new Wine_Category();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM wine_categories WHERE id = '" + id + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        ret.FillData(dr);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false);
                    }
                }
                con.Close();
            }

            return ret;
        }

        public static int GetAllRecordsForName(string name, out List<Wine_Category> categories )
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            int nbRecords = 0;
            categories = new List<Wine_Category>();
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM wine_categories WHERE name = '" + name + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Wine_Category ret = new Wine_Category();
                        ret.FillData(dr);
                        categories.Add(ret);
                        nbRecords++;
                    }
                }
                con.Close();
            }

            return nbRecords;
        }

        public static int GetAllRecordsForID(string id, out List<Wine_Category> categories)
        {
            categories = null;
            Wine_Category cat = GetRecordByID(id);
            if (cat.id == -1)
                return 0;
            return GetAllRecordsForName(cat.name, out categories);
        }

        public static List<Wine_Category> GetAllRecords(bool onlyStep1)
        {
            List<Wine_Category> categories = new List<Wine_Category>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM wine_categories";
                if (onlyStep1)
                    sqlQuery += " WHERE step=1";

                sqlQuery += " ORDER BY name";
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Wine_Category category = new Wine_Category();
                        category.FillData(dr);
                        categories.Add(category);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return categories;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Wine_Category
    {
        public int id;
        public string name;
        public float cost;
        public int step;
        public int days;
        public string symbol;

        public static Wine_Category GetRecordByID(string id)
        {
            Wine_Category ret = new Wine_Category();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM wine_categories WHERE id LIKE '" + id + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        int numValue;
                        float floatValue;
                        
                        bool parsed = Int32.TryParse(dr["id"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.id = numValue;

                        ret.name = dr["name"].ToString();

                        parsed = Single.TryParse(dr["cost"].ToString(), out floatValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.cost = floatValue;
                        
                        parsed = Int32.TryParse(dr["step"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.step = numValue;

                        parsed = Int32.TryParse(dr["days"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.days = numValue;

                        ret.symbol = dr["symbol"].ToString();
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

        public void SetValuesFromDataReader(MySqlDataReader dr)
        {
            int numValue;
            float floatValue;

            bool parsed = Int32.TryParse(dr["id"].ToString(), out numValue);
            System.Diagnostics.Debug.Assert(parsed);
            id = numValue;

            name = dr["name"].ToString();

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

        public static int GetAllRecordsForName(string name, out List<Wine_Category> categories )
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            int nbRecords = 0;
            categories = new List<Wine_Category>();
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM wine_categories WHERE name LIKE '" + name + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Wine_Category ret = new Wine_Category();
                        ret.SetValuesFromDataReader(dr);
                        categories.Add(ret);
                        nbRecords++;
                    }
                }
                con.Close();
            }

            return nbRecords;

        }
     }
}
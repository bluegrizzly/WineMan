using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Wine_Type
    {
        public int id=0;
        public string name;
        public bool active;
        public int brand_id;
        public int category_id;

        public static Wine_Type GetRecord(string name)
        {
            Wine_Type ret = new Wine_Type();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM wine_types WHERE id LIKE '" + name + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    if (dr.HasRows)
                    {
                        int numValue;
                        bool parsed = Int32.TryParse(dr["id"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.id = numValue;
                        ret.name = dr["name"].ToString();
                        ret.active = dr["active"].ToString() == "1" ? true : false;
                        parsed = Int32.TryParse(dr["brand_id"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.brand_id = numValue;
                        parsed = Int32.TryParse(dr["category_id"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.category_id = numValue;
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
     }
}
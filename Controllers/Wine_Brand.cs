using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Wine_Brand
    {
        public int id=0;
        public string name;
        public bool active;

        public static Wine_Brand GetRecord(string name)
        {
            Wine_Brand ret = new Wine_Brand();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("select DISTINCT * FROM wine_brands WHERE id LIKE '" + name + "'", con))
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Step : BaseController
    {
        public int id;
        public string name;
        public bool finalStep;

        public static Step GetRecord(string id)
        {
            Step ret = new Step();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("select DISTINCT * FROM steps WHERE id LIKE '" + id + "'", con))
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

                        parsed = Int32.TryParse(dr["final_step"].ToString(), out numValue);
                        System.Diagnostics.Debug.Assert(parsed);
                        ret.finalStep = numValue > 0 ? true : false;

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
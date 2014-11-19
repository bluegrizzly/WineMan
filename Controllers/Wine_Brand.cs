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

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                name = dr["name"].ToString();
                active = dr["active"].ToString() == "1" ? true : false;
            }
        }

        public static Wine_Brand GetRecordByID(string idStr)
        {
            Wine_Brand ret = new Wine_Brand();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("select DISTINCT * FROM wine_brands WHERE id LIKE '" + idStr + "'", con))
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

        public static List<Wine_Brand> GetAllRecords()
        {
            List<Wine_Brand> brands = new List<Wine_Brand>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM wine_brands", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Wine_Brand brand = new Wine_Brand();
                        brand.FillData(dr);
                        brands.Add(brand);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return brands;
        }
    }
}
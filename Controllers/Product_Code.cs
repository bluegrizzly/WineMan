using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Product_Code
    {
        const string c_dbName = "product_codes";
        public int id = 0;
        public string name;
        public string color;

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                name = dr["name"].ToString();
                color = dr["color"].ToString();
            }
        }

        public static Product_Code GetRecordByID(int id)
        {
            Product_Code ret = new Product_Code();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("select DISTINCT * FROM "+c_dbName+" WHERE id LIKE '" + id.ToString() + "'", con))
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

        public static List<Product_Code> GetAllRecords()
        {
            List<Product_Code> codes = new List<Product_Code>();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM "+ c_dbName, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Product_Code code = new Product_Code();
                        code.FillData(dr);
                        codes.Add(code);
                    }
                    dr.Close();
                }
                con.Close();
            }

            return codes;
        }
    }
}
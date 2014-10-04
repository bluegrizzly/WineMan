using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class CustomersHelper
    {
        public static List<string> GetSimilarCustomers(string inputName)
        {
            List<string> result = new List<string>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                bool found = false;
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT first_name, last_name FROM customers WHERE first_name LIKE '%" + inputName + "%'", con))
                {
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["first_name"].ToString() + "." + dr["last_name"].ToString());
                        found = true;
                    }
                    dr.Close();
                }
                if (!found)
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT first_name, last_name FROM customers WHERE last_name LIKE '%" + inputName + "%'", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            result.Add(dr["first_name"].ToString() + "." + dr["last_name"].ToString());
                            found = true;
                        }
                        dr.Close();
                    }
                }

                if (!found)
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT id, first_name, last_name FROM customers WHERE id LIKE '" + inputName + "'", con))
                        {
                            MySqlDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                result.Add(dr["first_name"].ToString() + "." + dr["last_name"].ToString());
                                found = true;
                            }
                            dr.Close();
                        }
                    }
                    catch { }
                }

                con.Close();
                return result;
            }
        }
    }
}
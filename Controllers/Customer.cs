using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Customer : BaseController
    {
        public int ID;
        public string FirstName;
        public string LastName;
        public string Addresse;
        public string Telephone;
        public string email;

        public Customer()
        {
            ID = -1;
        }

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out ID);
                System.Diagnostics.Debug.Assert(parsed);

                FirstName = dr["first_name"].ToString();
                LastName = dr["last_name"].ToString();
                Addresse = dr["address"].ToString();
                Telephone = dr["telephone"].ToString();
                email = dr["email"].ToString();
            }
        }

        public static Customer GetRecordByName(string firstName, string lastName)
        {
            Customer ret = new Customer();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM customers WHERE last_name LIKE '" + lastName + "' AND first_name LIKE '" + firstName + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    ret.FillData(dr);
                }
                con.Close();
            }
            return ret;
        }

        public static Customer GetRecordByID(string id)
        {
            Customer ret = new Customer();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT * FROM customers WHERE id LIKE '" + id + "'", con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    dr.Read();
                    ret.FillData(dr);
                }
                con.Close();
            }
            return ret;
        }

    }
}
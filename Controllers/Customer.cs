using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Customer : BaseController
    {
        public int id;
        public string first_name;
        public string last_name;
        public string addresse;
        public string city;
        public string province;
        public string postal_code;
        public string email;
        public string telephone;
        public string telephone_bur;
        public string telephone_fax;

        public Customer()
        {
            id = -1;
        }

        public string GetFullName()
        {
            return first_name + " " + last_name;
        }

        private void FillData(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                first_name = dr["first_name"].ToString();
                last_name = dr["last_name"].ToString();
                addresse = dr["address"].ToString();
                city = dr["city"].ToString();
                province = dr["province"].ToString();;
                postal_code = dr["postal_code"].ToString();
                postal_code.ToUpper();
                email = dr["email"].ToString(); ;
                telephone = dr["telephone"].ToString();
                telephone_bur = dr["telephone_bur"].ToString();
                telephone_fax = dr["telephone_fax"].ToString();
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
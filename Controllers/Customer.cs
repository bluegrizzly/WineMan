using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Customer : BaseController
    {
        public const string c_dbName = "customers";
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
        public int language;

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

                parsed = Int32.TryParse(dr["language"].ToString(), out language);
                System.Diagnostics.Debug.Assert(parsed);
            }
        }

        public static string GetDoublonValidationSqlQuery(System.Collections.Specialized.NameValueCollection forms)
        {
            string sqlCmd = "SELECT * FROM " + c_dbName+ " WHERE first_name='" + forms.Get("first_name").ToString() + "'" +
                " AND last_name='" + forms.Get("last_name").ToString() + "'" +
                " AND postal_code='" + forms.Get("postal_code").ToString() + "'";
            return sqlCmd;
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

        public static List<Customer> GetRecordBySqlQuery(string sqlQuery)
        {
            List<Customer> ret = new List<Customer>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                {
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Customer custo = new Customer();
                        custo.FillData(dr);
                        ret.Add(custo);
                    }
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

        public static string GetSqlQueryToResearchCustomers(string filterCustomer)
        {
            string sqlQuery = "";

            //try to see if it is the complete name
            string[] arrayString = filterCustomer.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arrayString.Count() == 2)
            {
                sqlQuery = "SELECT DISTINCT * FROM " + c_dbName + " WHERE first_name LIKE '%" + arrayString[0] + "%'" +
                    " AND last_name LIKE '%" + arrayString[1] + "%'";
            }
            else
            {
                sqlQuery = "SELECT DISTINCT * FROM " + c_dbName + " WHERE first_name LIKE '%" + filterCustomer + "%'" +
                    " OR last_name LIKE '%" + filterCustomer + "%'" +
                    " OR id = '" + filterCustomer + "'" +
                    " OR telephone LIKE '%" + filterCustomer + "%'" +
                    " ORDER BY last_name";
            }
            return sqlQuery;
        }

    }
}
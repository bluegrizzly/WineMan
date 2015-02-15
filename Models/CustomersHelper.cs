using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class CustomersHelper
    {
        public static List<string> GetSimilarCustomersString(string inputName)
        {
            List<Customer> customers =  GetSimilarCustomers(inputName);
            List<string> result = new List<string>();
            foreach (Customer custo in customers)
            {
                result.Add(custo.id.ToString() + ": " + custo.first_name + " " + custo.last_name + " [" + custo.telephone + "]");
            }
            return result;
        }

        public static List<Customer> GetSimilarCustomers(string inputName)
        {
            List<Customer> result = new List<Customer>();

            if (inputName == " ")
            {
                result = Customer.GetRecordBySqlQuery("SELECT DISTINCT * FROM customers ORDER BY last_name");
                return result;
            }

            // check first if we have a full name
            string[] arrayString = inputName.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arrayString.Count() == 2)
            {
                string sqlQuery = "SELECT DISTINCT * FROM customers WHERE first_name LIKE '%" + arrayString[0] + "%'" +
                    " AND last_name LIKE '%" + arrayString[1] + "%' ORDER BY last_name";
                result = Customer.GetRecordBySqlQuery(sqlQuery);
            }

            if (result.Count == 0)
                result = Customer.GetRecordBySqlQuery("SELECT DISTINCT * FROM customers WHERE first_name LIKE '%" + inputName + "%'" + " ORDER BY last_name");

            if (result.Count == 0)
                result = Customer.GetRecordBySqlQuery("SELECT DISTINCT * FROM customers WHERE last_name LIKE '%" + inputName + "%'" + " ORDER BY last_name");

            if (result.Count == 0)
                result = Customer.GetRecordBySqlQuery("SELECT DISTINCT * FROM customers WHERE id = '" + inputName + "'" + " ORDER BY last_name");

            if (result.Count == 0)
                result = Customer.GetRecordBySqlQuery("SELECT DISTINCT * FROM customers WHERE telephone LIKE '%" + inputName + "%'" + " ORDER BY last_name");

            return result;
        }
    }
}
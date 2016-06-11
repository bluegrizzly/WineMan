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
            string sqlQuery = Customer.GetSqlQueryToResearchCustomers(inputName);
            result = Customer.GetRecordBySqlQuery(sqlQuery);

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Transaction : BaseController
    {
        const string c_dbName = "transactions";
        public int id=-1;
        public int client_id;
        public int wine_brand_id;
        public int wine_type_id;
        public int wine_category_id;
        public DateTime date_creation;
        public DateTime date_bottling;
        public int bottling_station;
        public bool done;
        public List<TransactionStep> progress;

        private static System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");

        public enum FilterTypes
        {
            All,
            Today,
            ThisWeek,
            ThisMonth,
            Last4Weeks
        }

        private void FillRecord(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["client_id"].ToString(), out client_id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["wine_brand_id"].ToString(), out wine_brand_id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["wine_type_id"].ToString(), out wine_type_id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["wine_category_id"].ToString(), out wine_category_id);
                System.Diagnostics.Debug.Assert(parsed);

                DateTime.TryParse(dr["date_creation"].ToString(), out date_creation);
                DateTime.TryParse(dr["date_bottling"].ToString(), out date_bottling);

                parsed = Int32.TryParse(dr["bottling_station"].ToString(), out bottling_station);
                System.Diagnostics.Debug.Assert(parsed);

                int num;
                parsed = Int32.TryParse(dr["done"].ToString(), out num);
                done = num > 0 ? true : false;
                System.Diagnostics.Debug.Assert(parsed);

            }
        }

        public static bool CreateRecord(Transaction tx)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(0, true) + //id
                                    AddIntParameter(tx.client_id) +
                                    AddIntParameter(tx.wine_brand_id) +
                                    AddIntParameter(tx.wine_type_id) +
                                    AddIntParameter(tx.wine_category_id) +
                                    AddDateParameter(tx.date_creation) +
                                    AddDateParameter(tx.date_bottling) +
                                    AddIntParameter(tx.bottling_station) +
                                    AddIntParameter(0);  // Done
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO " + c_dbName + " VALUES (" + values + ")", con))
                    {
                        cmd.ExecuteNonQuery();

                        tx.id = (int)cmd.LastInsertedId;
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
                return false;
            }
        }

        public bool ModifyRecord()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(client_id, true, "client_id") +
                                    AddIntParameter(wine_brand_id, false, "wine_brand_id" ) +
                                    AddIntParameter(wine_type_id, false, "wine_type_id") +
                                    AddIntParameter(wine_category_id, false, "wine_category_id") +
                                    AddDateParameter(date_creation, false, "date_creation") +
                                    AddDateParameter(date_bottling, false, "date_bottling") +
                                    AddIntParameter(bottling_station, false, "bottling_station") +
                                    AddIntParameter(done?1:0, false, "done");  
                    con.Open();
                    string sqlQuery = "UPDATE " + c_dbName + " SET " + values + " WHERE id=" + id.ToString();
                    
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
                return false;
            }
        }

        public static Transaction GetRecord(int txID)
        {
            Transaction transaction = null;

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + c_dbName + " WHERE id = " + txID.ToString(), con))
                    {
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();
                        if (dr.HasRows)
                        {
                            transaction = new Transaction();
                            transaction.FillRecord(dr);
                        }

                        dr.Close();
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }

            return transaction;
        }

        public static List<Transaction> GetAllRecords(bool includeCompleted, FilterTypes filter, string filterCustomer)
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM " + c_dbName;
                    string sqlQueryWhere="";
                    if (!includeCompleted)
                        sqlQueryWhere += " WHERE done=0";

                    if (filter != FilterTypes.All)
                    {
                        if (sqlQueryWhere.Length != 0)
                            sqlQueryWhere += " AND";
                        else
                            sqlQueryWhere += " WHERE";

                        switch(filter)
                        { 
                            case FilterTypes.Today:
                                sqlQueryWhere += " CAST(date_creation AS DATE) = CAST(CURDATE() AS DATE)";
                                break;
                            case FilterTypes.ThisWeek:
                                sqlQueryWhere += " WEEK (date_creation) = WEEK( CURDATE() )  AND YEAR( date_creation) = YEAR( CURDATE() )";
                                break;
                            case FilterTypes.ThisMonth:
                                sqlQueryWhere += " MONTH (date_creation) = MONTH( CURDATE() )  AND YEAR( date_creation) = YEAR( CURDATE() )";
                                break;
                            case FilterTypes.Last4Weeks:
                                sqlQueryWhere += " WHERE date_creation > DATE_ADD( now( ) , INTERVAL -1 MONTH ) ";
                                break;
                        }
                    }

                    if (filterCustomer != null && filterCustomer.Length > 0)
                    {
                        List<Customer> customers = CustomersHelper.GetSimilarCustomers(filterCustomer);

                        bool firstCase=true;
                        foreach (Customer custo in customers)
                        {
                            if (firstCase)
                            {
                                if (sqlQueryWhere.Length != 0)
                                    sqlQueryWhere += " AND (";
                                else
                                    sqlQueryWhere += " WHERE (";
                                firstCase = false;
                            }
                            else
                                sqlQueryWhere += (" OR");

                            sqlQueryWhere += " client_id = " + custo.id.ToString();
                        }
                        if (customers.Count > 0)
                            sqlQueryWhere += ")";
                        
                    }

                    sqlQuery += sqlQueryWhere;

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            Transaction tx = new Transaction();
                            tx.FillRecord(dr);
                            transactions.Add(tx);
                        }

                        dr.Close();
                    }
                    con.Close();
                }
            }
            catch { }

            return transactions;
        }


        public static List<Transaction> GetRecords(DateTime date)
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string dateStr = date.ToString("yyyy-MM-dd");
                    dateStr += " %";
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + c_dbName + " WHERE date_bottling LIKE '" + dateStr + "'", con))
                    {
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            Transaction tx = new Transaction();
                            tx.FillRecord(dr);
                            transactions.Add(tx);
                        }

                        dr.Close();
                    }
                    con.Close();
                }
            }
            catch { }

            return transactions;
        }

        public static List<Transaction> GetAllnotDone()
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + c_dbName + " WHERE done = 0", con))
                    {
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            Transaction tx = new Transaction();
                            tx.FillRecord(dr);
                            transactions.Add(tx);
                        }

                        dr.Close();
                    }
                    con.Close();
                }
            }
            catch { }

            return transactions;
        }
    }
}
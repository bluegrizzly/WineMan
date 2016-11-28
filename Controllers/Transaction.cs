using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace WineMan
{
    public class TransactionForTableView
    {
        public string id;
        public string customer;
        public string wine_brand_id;
    }

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
        public string comments;
        public string location;
        public int product_code=0;
        // Properties got from other tables
        public string CustomerName;
        public string CustomerTelephone;
        public string BrandName;
        public string TypeName;
        public string CategoryName;
        public string Symbol;


        private static CultureInfo m_Culture = new System.Globalization.CultureInfo("en-US");

        public enum FilterTypes
        {
            All,
            Today,
            ThisWeek,
            ThisMonth,
            Last4Weeks
        }

        public enum DateKind
        {
            Unknown,
            Creation,
            Bottling
        }

        private void FillRecord(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                id = dr.GetInt32("id");
                client_id = dr.GetInt32("client_id");
                wine_brand_id = dr.GetInt32("wine_brand_id");
                wine_type_id = dr.GetInt32("wine_type_id");
                wine_category_id = dr.GetInt32("wine_category_id");

                string aa = dr["date_creation"].ToString();
                bool parsed = DateTime.TryParseExact(dr["date_creation"].ToString(), "G", m_Culture, DateTimeStyles.None, out date_creation);
                System.Diagnostics.Debug.Assert(parsed);

                aa = dr["date_bottling"].ToString();
                parsed = DateTime.TryParseExact(dr["date_bottling"].ToString(), "G", m_Culture, DateTimeStyles.None, out date_bottling);
                System.Diagnostics.Debug.Assert(parsed);

                bottling_station = dr.GetInt32("bottling_station");
                done = dr.GetBoolean("done");

                comments = dr["comments"].ToString();
                location = dr["location"].ToString();
                product_code = dr.GetInt16("product_code");  // 32767 is Int16Max

                try
                {
                    CustomerName = dr.GetString("CustomerFirstName") + " " + dr.GetString("CustomerLastName");
                    CustomerTelephone = dr.GetString("CustomerTel");
                    BrandName = dr.GetString("BrandName");
                    TypeName = dr.GetString("TypeName");
                    CategoryName = dr.GetString("CategoryName");
                    Symbol = dr.GetString("Symbol");
                }
                catch (Exception) { }
            }
        }

        private static int GetNextAvailableID()
        {
             Settings settings = new Settings();
            int txID= settings.transaction_starting_id;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    while (true)
                    { 
                        string sqlQuery = "SELECT id FROM " + c_dbName + " WHERE id = " + txID;
                        using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                        {
                            if (cmd.ExecuteScalar() != null)
                                txID++;
                            else
                                break;
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }
            return txID;
        }

        public static bool CreateRecord(Transaction tx)
        {
            try
            {
                // Find a next valid record that start with the id
                int initialID = GetNextAvailableID();

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(initialID, true) + //id
                                    AddIntParameter(tx.client_id) +
                                    AddIntParameter(tx.wine_brand_id) +
                                    AddIntParameter(tx.wine_type_id) +
                                    AddIntParameter(tx.wine_category_id) +
                                    AddDateParameter(tx.date_creation) +
                                    AddDateParameter(tx.date_bottling) +
                                    AddIntParameter(tx.bottling_station) +
                                    AddIntParameter(0) + 
                                    AddStringParameter(tx.comments) +
                                    AddStringParameter(tx.location) +
                                    AddIntParameter(tx.product_code);
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
                                    AddIntParameter(done?1:0, false, "done") +
                                    AddStringParameter(comments,false, "comments") +
                                    AddStringParameter(location, false, "location") +
                                    AddIntParameter(product_code, false, "product_code");
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
            return GetRecord(txID.ToString());
        }
        public static Transaction GetRecord(string txID)
        {
            Transaction transaction = null;

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + c_dbName + " WHERE id = " + txID, con))
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

        public static List<Transaction> GetAllRecords(
            EShow showFilter, 
            FilterTypes filter, 
            string filterCustomer, 
            DateTime dateStart, 
            DateTime dateEnd,
            bool showReadyOnly = false,
            Transaction.DateKind dateKind = Transaction.DateKind.Unknown,
            int txID = -1)
        {
            string sqlQuery = "SELECT *, customers.last_name as CustomerLastName, " +
                               "customers.first_name as CustomerFirstName, " +
                               "wine_categories.symbol as symbol, " +
                               "wine_brands.name as BrandName, " +
                               "wine_types.name as TypeName, " +
                               "wine_categories.name as CategoryName, " +
                               "customers.telephone as CustomerTel " +
                               "FROM " + c_dbName;

            string sqlQueryWhere = "";
            // if there is a tx id , we want only this one.
            if (txID > 0)
            {
                sqlQueryWhere = " WHERE transactions.id='" + txID.ToString() + "'";
            }
            else
            {
                // WHERE : done
                if (showFilter == EShow.Show_NotDone)
                    sqlQueryWhere += " WHERE transactions.done=0";
                else if (showFilter == EShow.Show_Done)
                    sqlQueryWhere += " WHERE transactions.done=1";

                // WHERE : filter type
                if (filter != FilterTypes.All)
                {
                    if (sqlQueryWhere.Length != 0)
                        sqlQueryWhere += " AND";
                    else
                        sqlQueryWhere += " WHERE";

                    switch (filter)
                    {
                        case FilterTypes.Today:
                            sqlQueryWhere += " CAST(transactions.date_creation AS DATE) = CAST(CURDATE() AS DATE)";
                            break;
                        case FilterTypes.ThisWeek:
                            sqlQueryWhere += " WEEK (transactions.date_creation) = WEEK( CURDATE() )  AND YEAR( transactions.date_creation) = YEAR( CURDATE() )";
                            break;
                        case FilterTypes.ThisMonth:
                            sqlQueryWhere += " MONTH (transactions.date_creation) = MONTH( CURDATE() )  AND YEAR( transactions.date_creation) = YEAR( CURDATE() )";
                            break;
                        case FilterTypes.Last4Weeks:
                            sqlQueryWhere += " WHERE transactions.date_creation > DATE_ADD( now( ) , INTERVAL -1 MONTH ) ";
                            break;
                    }
                }
                else if (dateStart > DateTime.MinValue || dateEnd < DateTime.MaxValue)
                {
                    // WHERE : dates
                    if (sqlQueryWhere.Length != 0)
                        sqlQueryWhere += " AND";
                    else
                        sqlQueryWhere += " WHERE";
                    string dateStr = dateStart.Year.ToString() + "-" + dateStart.Month.ToString() + "-" + dateStart.Day.ToString() + " %";
                    string dateStrEnd = dateEnd.Year.ToString() + "-" + dateEnd.Month.ToString() + "-" + dateEnd.Day.ToString() + " %";

                    if (dateKind == DateKind.Bottling)
                        sqlQueryWhere += " (transactions.date_bottling BETWEEN '" + dateStr + "'" + " AND '" + dateStrEnd + "')";
                    else //if (dateKind == DateKind.Creation)
                        sqlQueryWhere += " (transactions.date_creation BETWEEN '" + dateStr + "'" + " AND '" + dateStrEnd + "')";
                }

                // WHERE : customer
                if (filterCustomer != null && filterCustomer.Length > 0)
                {
                    if (sqlQueryWhere.Length != 0)
                        sqlQueryWhere += " AND";
                    else
                        sqlQueryWhere += " WHERE";

                    sqlQueryWhere += GetSqlQueryToResearchCustomers(filterCustomer);
                }
            }

            string innerjoinStr = " INNER JOIN customers ON customers.id = transactions.client_id ";
            innerjoinStr += "INNER JOIN wine_brands ON wine_brands.id = transactions.wine_brand_id ";
            innerjoinStr += "INNER JOIN wine_types ON wine_types.id = transactions.wine_type_id ";
            innerjoinStr += "INNER JOIN wine_categories ON wine_categories.id = transactions.wine_category_id ";

            sqlQuery += innerjoinStr + sqlQueryWhere + " ORDER BY transactions.id ASC";

            if (showReadyOnly && txID <= 0)
            {
                List<Transaction> alltx = GetRecordsFromSqlQuery(sqlQuery);
                List<Transaction> alltxRet = new List<Transaction>();
                foreach (Transaction tx in alltx)
                {
                    int nbStepDone = 0;
                    int nbStepTotal = 0;
                    tx.AreAllStepDone(out nbStepDone, out nbStepTotal);
                    if (nbStepTotal == nbStepDone)
                        alltxRet.Add(tx);
                }
                return alltxRet;
            }
            else
                return GetRecordsFromSqlQuery(sqlQuery);

        }

        private static string GetSqlQueryToResearchCustomers(string filterCustomer)
        {
            if (filterCustomer.Length == 0)
                return "";

            string sqlQuery = "";

            sqlQuery = Customer.GetSqlQueryToResearchCustomers(filterCustomer);
            List<Customer> customers = Customer.GetRecordBySqlQuery(sqlQuery);

            sqlQuery = "";
            bool needOr=false;

            if (customers.Count > 0)
                sqlQuery += " (";
            foreach (Customer customer in customers)
            {
                if (needOr)
                {
                    sqlQuery += " OR ";
                }
                sqlQuery += " transactions.client_id=" + customer.id.ToString();
                needOr = true;
            }
            if (customers.Count > 0)
                sqlQuery += " )";


            return sqlQuery;
        }

        // get the records with bottling dates
        public static List<Transaction> GetRecords(DateTime bottlingDate)
        {
            string dateStr = bottlingDate.ToString("yyyy-MM-dd");
            dateStr += " %";
            return GetRecordsFromSqlQuery ( "SELECT * FROM " + c_dbName + " WHERE date_bottling LIKE '" + dateStr + "'");
        }

        public static List<Transaction> GetAllRecordsWithWineCategory(string categoryID)
        {
            return GetRecordsFromSqlQuery("SELECT * FROM " + c_dbName + " WHERE wine_category_id=" + categoryID);
        }
        public static List<Transaction> GetAllRecordsWithWineBrand(string brandID)
        {
            return GetRecordsFromSqlQuery("SELECT * FROM " + c_dbName + " WHERE wine_brand_id=" + brandID);
        }
        public static List<Transaction> GetAllRecordsWithWineType(string typeID)
        {
            return GetRecordsFromSqlQuery("SELECT * FROM " + c_dbName + " WHERE wine_type_id=" + typeID);
        }

        public static List<Transaction> GetAllnotDone()
        {
            return GetRecordsFromSqlQuery("SELECT * FROM " + c_dbName + " WHERE done = 0");
        }

        public static List<Transaction> GetAllRecordsCreatedInMonth(DateTime date)
        {
            string sqlQuery = "SELECT * FROM " + c_dbName;

            DateTime dateStart = new DateTime (date.Year, date.Month, 1);
            string dateStrStart = dateStart.Year.ToString() + "-" + dateStart.Month.ToString() + "-" + "1" + " %";
            DateTime dateEnd = dateStart.AddMonths(1);
            string dateStrEnd = dateEnd.Year.ToString() + "-" + dateEnd.Month.ToString() + "-" + "1" + " %";

            sqlQuery += " WHERE (date_creation >= '" + dateStrStart + "'" + " AND date_creation < '" + dateStrEnd + "')";
            
            return GetRecordsFromSqlQuery(sqlQuery);
        }

        public static int GetAllRecordsForCustomer(string idToDelete, out List<Transaction> allTx)
        {
            string sqlQuery = "SELECT * FROM " + c_dbName + " WHERE client_id ='" + idToDelete + "'";
            allTx = GetRecordsFromSqlQuery(sqlQuery);
            return allTx.Count;
        }

        public static List<Transaction> GetRecordsFromSqlQuery(string sqlQuery)
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
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

        public bool AreAllStepDone(out int nbDone, out int total)
        {
            List<TransactionStep> steps = TransactionStep.GetRecordsForTx(id);
            nbDone = 0;
            total = steps.Count;

            foreach(TransactionStep step in steps)
            {
                Step productionStep = Step.GetRecordById(step.step_id.ToString());

                // If the step is not required odn't count it in the total
                if (productionStep.required_for_completion == 0)
                    total--;
                else if (step.done > 0)
                    nbDone++;
            }
            if (nbDone == total)
                return true;
            return false;
        }

        public bool IsStarted()
        {
            // Return true if the yeast is done 
            List<TransactionStep> steps = TransactionStep.GetRecordsForTx(id);
            TransactionStep yeast = steps[0];
            if (yeast.done > 0 )
                return true;

            return false;
        }
    }
}
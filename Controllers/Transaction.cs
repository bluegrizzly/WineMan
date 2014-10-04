using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class Transaction : BaseController
    {
        public int id;
        public int client_id;
        public int wine_brand_id;
        public int wine_type_id;
        public int wine_category_id;
        public DateTime date_creation;
        public DateTime date_bottling;
        public bool done;
        public int bottling_station;

        private static System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");

        public static bool CreateRecord(Transaction tx)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(tx.id, true) +
                                    AddIntParameter(tx.client_id) +
                                    AddIntParameter(tx.wine_brand_id) +
                                    AddIntParameter(tx.wine_type_id) +
                                    AddIntParameter(tx.wine_category_id) +
                                    AddDateParameter(tx.date_creation) +
                                    AddDateParameter(tx.date_bottling) +
                                    AddIntParameter(0) +  // Done
                                    AddIntParameter(tx.bottling_station);
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO transactions  VALUES (" + values + ")", con))
                    {
                        cmd.ExecuteNonQuery();

                        tx.id = (int)cmd.LastInsertedId;
                    }
                    con.Close();
                }
                return true;
            }
            catch /*(Exception e)*/{ return false; }
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

                DateTime.TryParse( dr["date_creation"].ToString(), out date_creation);
                DateTime.TryParse(dr["date_bottling"].ToString(), out date_bottling);

                int num;
                parsed = Int32.TryParse(dr["done"].ToString(), out num);
                done = num > 0 ? true : false;
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["bottling_station"].ToString(), out bottling_station);
                System.Diagnostics.Debug.Assert(parsed);
            }
        }

        public static List<Transaction> GetRecords(DateTime date)
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string dateStr = date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " %";
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM transactions WHERE date_bottling LIKE '" + dateStr + "'", con))
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
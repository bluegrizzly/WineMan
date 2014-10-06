﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class TransactionStep : BaseController
    {
        const string c_dbName = "transaction_step";
        public int id=-1;
        public int transaction_id;
        public int step_id;
        public DateTime date;
        public int done;

        public static bool CreateRecord(TransactionStep txStep)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(txStep.transaction_id, true) +
                                    AddIntParameter(txStep.step_id) +
                                    AddDateParameter(txStep.date) +
                                    AddIntParameter(txStep.done);
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO "+c_dbName+" VALUES (" + values + ")", con))
                    {
                        cmd.ExecuteNonQuery();
                        txStep.id = (int)cmd.LastInsertedId;
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

        private void FillRecord(MySqlDataReader dr)
        {
            if (dr.HasRows)
            {
                bool parsed = Int32.TryParse(dr["id"].ToString(), out id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["transaction_id"].ToString(), out transaction_id);
                System.Diagnostics.Debug.Assert(parsed);

                parsed = Int32.TryParse(dr["step_id"].ToString(), out step_id);
                System.Diagnostics.Debug.Assert(parsed);

                DateTime.TryParse(dr["date"].ToString(), out date);

                parsed = Int32.TryParse(dr["done"].ToString(), out done);
                System.Diagnostics.Debug.Assert(parsed);
            }
        }

        public static List<TransactionStep> GetRecords(DateTime date)
        {
            List<TransactionStep> txSteps = new List<TransactionStep>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string dateStr = date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " %";
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM "+c_dbName+" WHERE date LIKE '" + dateStr + "'", con))
                    {
                        con.Open();
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            TransactionStep txStep = new TransactionStep();
                            txStep.FillRecord(dr);
                            txSteps.Add(txStep);
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

            return txSteps;
        }


   }
}
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

        public static bool CreateRecord(TransactionStep txStep)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string values = AddIntParameter(0, true) + 
                                    AddIntParameter(txStep.transaction_id) +
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

        public static string BuildSQLQuery(DateTime date, DateTime dateEnd, EShow showdone, int stepId=-1, string txID=null)
        {
            string dateStr = date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " %";
            string dateStrEnd = dateEnd.Year.ToString() + "-" + dateEnd.Month.ToString() + "-" + dateEnd.Day.ToString() + " %";
            string sqlQuery = "SELECT * FROM " + c_dbName + " WHERE (date BETWEEN '" + dateStr + "'" + " AND '" + dateStrEnd + "')";
            if (showdone == EShow.Show_Done)
                sqlQuery += " AND done=1";
            else if (showdone == EShow.Show_NotDone)
                sqlQuery += " AND done=0";

            if (stepId>=0)
                sqlQuery += " AND step_id="+stepId ;

            if (txID != null && txID != "")
                sqlQuery += " AND transaction_id=" + txID;

            return sqlQuery;
        }

        public static List<TransactionStep> GetRecords(DateTime date, DateTime dateEnd, EShow showdone, int stepId=-1, string txID=null)
        {
            List<TransactionStep> txSteps = new List<TransactionStep>();

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = BuildSQLQuery(date, dateEnd, showdone, stepId, txID);

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
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

        public static List<TransactionStep> GetRecordsForTx(int txID)
        {
            List<TransactionStep> txSteps = new List<TransactionStep>();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM " + c_dbName + " WHERE transaction_id =" + txID.ToString();

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
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

        public static bool DeleteTxRecords(int txID)
        {
            bool ret = false;
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = "DELETE FROM " + c_dbName + " WHERE transaction_id=" + txID.ToString();

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        int result = cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }

            return ret;
        }

        public string GetStepName()
        {
            return Step.GetStepName(step_id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;

namespace WineMan
{
    public class TransactionsHelper
    {
        List<Step> m_Steps = new List<Step>();
        const string c_dbTransactionStepName = "transaction_step";

        public List<TransactionStep> GetAllStepsOfThisDay(DateTime date)
        {
            List<TransactionStep> steps = new List<TransactionStep> ();
            steps = TransactionStep.GetRecords(date);
            return steps;
        }

        public string GetStepName(int stepid)
        {
            if (m_Steps.Count == 0)
                m_Steps = Step.GetAllRecords();
            foreach(Step step in m_Steps)
            {
                if (step.id == stepid)
                    return step.name;
            }
            
            System.Diagnostics.Debug.Assert(false);
            return "";
        }
        // Creates all TransactionSteps for this transaction
        static public bool CreateStepsRecord(Transaction tx)
        {
            List<Wine_Category> categories;
            int nbRecords = Wine_Category.GetAllRecordsForID(tx.wine_category_id.ToString(), out categories);

            System.Diagnostics.Debug.Assert(categories.Count > 0);
            if (categories.Count == 0)
                return false;
            foreach (Wine_Category cat in categories)
            {
                // Create a transaction step for each step in the category (the recipes)
                TransactionStep txStep = new TransactionStep();
                txStep.done = 0; // false
                txStep.transaction_id = tx.id;
                txStep.step_id = cat.step;
                txStep.date = tx.date_creation.AddDays(cat.days);
                if (!TransactionStep.CreateRecord(txStep))
                {
                    System.Diagnostics.Debug.Assert(false);
                    return false;
                }
            }
            return true;
        }

        public void GetTransactionStepJSONRecords(HttpContext context, List<TransactionStep> steps)
        {
            string retString = @"{";

            int iterNb = 0;
            int nbRows = steps.Count;
            retString += @"""total"": """ + nbRows.ToString() + @""",";
            retString += @"""page"": ""1"",";
            retString += @"""records"": """ + nbRows.ToString() + @""",";
            retString += @"""rows"" : [ ";
            foreach (TransactionStep step in steps)
            {
                if (iterNb != 0)
                    retString += ",";

                retString += @"{""id"":" + iterNb + @", ""cell"" :[";
                iterNb++;

                retString += @"""" + step.id.ToString() + @""",";
                retString += @"""" + step.transaction_id.ToString() + @""",";
                retString += @"""" + GetStepName(step.step_id) + @""",";
                if (step.done == 0)
                    retString += @"""No""" ;  
                else
                    retString += @"""Yes""";  
                retString += "]}";
            }
            retString += "]}";
            context.Response.Write(retString);
        }

        public bool SetTransactionStepToDone(List<string> ids)
        {
            string sqlQuery = "UPDATE " + c_dbTransactionStepName + " SET done=1 WHERE ";
            bool res = false;
            bool first =true;

            foreach (string id in ids)
            {
                if (!first)
                {
                    sqlQuery += " OR ";
                    first = false;
                }

                sqlQuery += "id=" + id;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                // Open the SqlConnection.
                con.Open();

                using (MySqlCommand command = new MySqlCommand(sqlQuery, con))
                {
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        res = true;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Assert(false, e.Message);
                        res = false;
                    }
                }
            }
            return res;
        }
    }
}
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
        const string c_dbTransactionName = "transactions";

        public List<TransactionStep> GetAllStepsOfThisDay(DateTime dateStart, DateTime dateEnd, bool showlate, EShow showdone, int stepID, string txID)
        {
            List<TransactionStep> steps = new List<TransactionStep> ();
            steps = TransactionStep.GetRecords(dateStart, dateEnd, showdone, stepID, txID);

            if (showlate)
            {
                List<TransactionStep> stepsLate;
                DateTime oldDate = new DateTime (1,1,1);
                stepsLate = TransactionStep.GetRecords(oldDate, dateStart, EShow.Show_NotDone, stepID, txID);
                steps.AddRange(stepsLate);
            }
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
            return "not found";
        }

        public string GetBrandName(int brandId)
        {
            Wine_Brand brand = Wine_Brand.GetRecordByID(brandId.ToString());
            if (brand != null)
                return brand.name;
            else
                return "not found";
        }

        public string GetTypeName(int brandId)
        {
            Wine_Type type = Wine_Type.GetRecordByID(brandId.ToString());
            if (type != null)
                return type.name;
            else
                return "not found";
        }

        public string GetCategoryName(int categoryId)
        {
            Wine_Category category = Wine_Category.GetRecordByID(categoryId.ToString());
            if (category != null)
                return category.name;
            else
                return "not found";
        }

        // Get all transactions in JSON format for the grid
        public void GetTransactionJSONRecords(HttpContext context, List<Transaction> allTx)
        {
            string retString = @"{";

            int iterNb = 0;
            int nbRows = allTx.Count;
            retString += @"""total"": """ + nbRows.ToString() + @""",";
            retString += @"""page"": ""1"",";
            retString += @"""records"": """ + nbRows.ToString() + @""",";
            retString += @"""rows"" : [ ";
            foreach (Transaction tx in allTx)
            {
                if (iterNb != 0)
                    retString += ",";

                retString += @"{""id"":" + iterNb + @", ""cell"" :[";
                iterNb++;
                Customer customer = Customer.GetRecordByID(tx.client_id.ToString());

                retString += @"""" + tx.id.ToString() + @""", ";
                retString += @"""" + customer.first_name + " " + customer.last_name + @""", ";
                retString += @"""" + GetBrandName(tx.wine_brand_id) + @""", ";
                retString += @"""" + GetTypeName(tx.wine_type_id) + @""", ";
                retString += @"""" + GetCategoryName(tx.wine_category_id) + @""", ";
                retString += @"""" + tx.date_creation.ToString() + @""", ";
                retString += @"""" + tx.date_bottling.ToString() + @""", ";
                retString += @"""" + tx.bottling_station.ToString() + @""", ";
                retString += @"""" + tx.done.ToString() + @"""";
                retString += "]}";
            }
            retString += "]}";
            context.Response.Write(retString);
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
                txStep.date = new DateTime(tx.date_creation.AddDays(cat.days).Year, tx.date_creation.AddDays(cat.days).Month, tx.date_creation.AddDays(cat.days).Day);
                
                if (!TransactionStep.CreateRecord(txStep))
                {
                    System.Diagnostics.Debug.Assert(false);
                    return false;
                }
            }
            return true;
        }

        static public DateTime GetFinalStepDate(Transaction tx, DateTime date, TransactionStep txStep)
        {
            DateTime finalDate = date;
            List<Wine_Category> categories;
            Wine_Category.GetAllRecordsForID(tx.wine_category_id.ToString(), out categories);
            int nbDays = 0;
            foreach (Wine_Category category in categories)
            {
                if (category.step < txStep.step_id)
                    continue; // we don't care about passed steps
                else if (category.step == txStep.step_id)
                {
                    finalDate = date;
                    nbDays = category.days;
                }
                else
                {
                    finalDate = finalDate.AddDays(category.days - nbDays);
                    nbDays = category.days;
                }
            }
            return finalDate;
        }

        static public bool UpdateStepsRecordDate(Transaction tx, int stepID, DateTime date, out string datesAdjusted)
        {
            List<Wine_Category> categories;
            Wine_Category.GetAllRecordsForID(tx.wine_category_id.ToString(), out categories);
            int nbDays = 0;
            datesAdjusted = null;

            foreach (Wine_Category category in categories)
            {
                if (category.step < stepID)
                    continue; // we don't care about passed steps
                else 
                {
                    TransactionStep txStep = TransactionStep.GetRecordForTx(tx.id, category.step);

                    DateTime newCurrentDate = date;
                    if (category.step == stepID)
                        nbDays = category.days;
                    else
                    {
                        newCurrentDate = newCurrentDate.AddDays(category.days - nbDays);
                        nbDays = category.days;
                        // do not change date if the new date is before the current one.
                        if (newCurrentDate < txStep.date)
                            continue;
                    }
                    DateTime oldDate = txStep.date;
                    txStep.date = newCurrentDate;
                    if (txStep.UpdateDate(newCurrentDate) == false)
                        return false; // TODO revert other changes

                    // notify that we changed a ulterior date
                    if (category.step != stepID)
                        datesAdjusted += "\\n * [Step:" + Step.GetStepName(category.step) + " Old Date:" + oldDate.ToString("MMM-dd-yyyy") + " New Date:" + txStep.date.ToString("MMM-dd-yyyy");
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
                Transaction tx = Transaction.GetRecord(step.transaction_id);
                if (tx == null)
                    continue;

                Customer customer = Customer.GetRecordByID(tx.client_id.ToString());

                retString += @"""" + step.id.ToString() + @""",";
                retString += @"""" + step.transaction_id.ToString() + @""",";
                retString += @"""" + step.date.ToString() + @""",";
                retString += @"""" + GetStepName(step.step_id) + @""",";
                retString += @"""" + GetBrandName(tx.wine_brand_id) + @""",";
                retString += @"""" + GetTypeName(tx.wine_type_id) + @""",";
                retString += @"""" + customer.first_name + " " + customer.last_name + @""",";
                retString += @"""" + customer.telephone + @""",";
                retString += @"""" + step.done.ToString() + @"""";  
                retString += "]}";
            }
            retString += "]}";
            context.Response.Write(retString);
        }

        public void GetTransactionToCompleteJSONRecords(HttpContext context, List<Transaction> transactions)
        {
            string retString = @"{";

            int iterNb = 0;
            int nbRows = transactions.Count;
            retString += @"""total"": """ + nbRows.ToString() + @""",";
            retString += @"""page"": ""1"",";
            retString += @"""records"": """ + nbRows.ToString() + @""",";
            retString += @"""rows"" : [ ";
            foreach (Transaction tx in transactions)
            {
                if (iterNb != 0)
                    retString += ",";

                retString += @"{""id"":" + iterNb + @", ""cell"" :[";
                iterNb++;
                Customer customer = Customer.GetRecordByID(tx.client_id.ToString());

                int nbStepDone = 0;
                int nbStepTotal = 0;
                tx.AreAllStepDone(out nbStepDone, out nbStepTotal);
                string stepsDone = nbStepDone.ToString() + "/" + nbStepTotal.ToString();

                retString += @"""" + tx.id.ToString() + @""",";
                retString += @"""" + customer.first_name + " " + customer.last_name + @""",";
                retString += @"""" + GetBrandName(tx.wine_brand_id) + @""",";
                retString += @"""" + GetTypeName(tx.wine_type_id) + @""",";
                retString += @"""" + GetCategoryName(tx.wine_category_id) + @""",";
                retString += @"""" + tx.date_creation.ToString() + @""",";
                retString += @"""" + stepsDone + @""",";
                retString += @"""" + tx.done.ToString() + @"""";
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
                }
                first = false;

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

        public bool SetTransactionToDone(List<string> ids)
        {

            string sqlQuery = "UPDATE " + c_dbTransactionName + " SET done=1 WHERE ";
            bool res = false;
            bool first = true;

            List<int> txNotDone = new List<int>();
            
            foreach (string id in ids)
            {
                // First validate if allstesps are done 
                int nbStepDone = 0;
                int nbStepTotal = 0;
                int txid=0;
                Int32.TryParse(id, out txid);
                Transaction tx = Transaction.GetRecord(txid);
                if (!tx.AreAllStepDone(out nbStepDone, out nbStepTotal))
                {
                    txNotDone.Add(txid);
                    continue;
                }
                 
                if (!first)
                {
                    sqlQuery += " OR ";
                }
                first = false;

                sqlQuery += "id=" + id;
            }

            if (txNotDone.Count > 0)
            {
                //Utils.MessageBox(HttpContext.Current.Handler as System.Web.UI.Page, "Cannot complete these transactions because they still have steps not completed. :");
            }
            if (first)
                return false; // nothing processed

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
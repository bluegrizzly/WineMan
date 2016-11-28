using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace WineMan
{
    public class TransactionsHelper
    {
        List<Step> m_Steps = new List<Step>();
        const string c_dbTransactionStepName = "transaction_step";
        const string c_dbTransactionName = "transactions";

        public List<TransactionStep> GetAllStepsOfThisDay(DateTime dateStart, DateTime dateEnd, bool showlate, EShow showdone, int stepID, string txID, string customer)
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

            // Filter the customer
            if (customer != null && customer.Length > 0)
            {
                List<Customer> customers = CustomersHelper.GetSimilarCustomers(customer);

                for (int i=steps.Count-1; i >=0; i--)
                {
                    TransactionStep step = steps[i];
                    bool found = false;
                    Transaction tx = Transaction.GetRecord(step.transaction_id);
                    if (tx != null)
                    {
                        foreach (Customer cus in customers)
                        {
                            if (tx.client_id == cus.id)
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        steps.RemoveAt(i);
                    }
                }
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
        public void GetTransactionJSONRecords(HttpContext context, List<Transaction> allTx, bool forTxDoneScreen)
        {
            int iterNb = 0;
            int nbRows = allTx.Count;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("total");
                jsonWriter.WriteValue(nbRows);
                jsonWriter.WritePropertyName("page");
                jsonWriter.WriteValue(1);
                jsonWriter.WritePropertyName("records");
                jsonWriter.WriteValue(nbRows);
                jsonWriter.WritePropertyName("rows");
                jsonWriter.WriteStartArray();

                foreach (Transaction tx in allTx)
                {
                    if (forTxDoneScreen)
                    {
                        int nbStepDone = 0;
                        int nbStepTotal = 0;
                        tx.AreAllStepDone(out nbStepDone, out nbStepTotal);
                        string stepsDone = nbStepDone.ToString() + "/" + nbStepTotal.ToString();

                        jsonWriter.WriteStartArray();
                        jsonWriter.WriteValue(tx.id.ToString());
                        jsonWriter.WriteValue(tx.CustomerName);
                        jsonWriter.WriteValue(tx.Symbol);
                        jsonWriter.WriteValue(tx.BrandName);
                        jsonWriter.WriteValue(tx.TypeName);
                        jsonWriter.WriteValue(tx.CategoryName);
                        jsonWriter.WriteValue(tx.date_creation.ToString());
                        jsonWriter.WriteValue(tx.date_bottling.ToString());
                        jsonWriter.WriteValue(stepsDone);
                        jsonWriter.WriteValue(tx.location.ToString());
                        jsonWriter.WriteValue(tx.done.ToString());
                    }
                    else
                    {
                        jsonWriter.WriteStartArray();
                        jsonWriter.WriteValue(tx.id.ToString());
                        jsonWriter.WriteValue(tx.CustomerName);
                        jsonWriter.WriteValue(tx.Symbol);
                        jsonWriter.WriteValue(tx.BrandName);
                        jsonWriter.WriteValue(tx.TypeName);
                        jsonWriter.WriteValue(tx.CategoryName);
                        jsonWriter.WriteValue(tx.date_creation.ToString());
                        jsonWriter.WriteValue(tx.date_bottling.ToString());
                        jsonWriter.WriteValue(tx.bottling_station.ToString());
                        jsonWriter.WriteValue(tx.location.ToString());
                        jsonWriter.WriteValue(tx.product_code == Int16.MaxValue ? "" : tx.product_code.ToString());
                        jsonWriter.WriteValue(tx.done.ToString());
                    }

                    jsonWriter.WriteEndArray();
                    iterNb++;
                }

                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject();
                string retString = sb.ToString();
                context.Response.Write(retString);
            }
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

        // Return true if the transaction steps dates doesn't follow the recipes
        static public bool CheckBrokenRecipes(Transaction tx)
        {
            List<Wine_Category> categories;
            Wine_Category.GetAllRecordsForID(tx.wine_category_id.ToString(), out categories);
            List<TransactionStep> txSteps = TransactionStep.GetRecordsForTx(tx.id);
            DateTime startDate = txSteps[0].date;

            foreach (TransactionStep step in txSteps)
            {
                Wine_Category category = null;
                foreach (Wine_Category cat in categories)
                {
                    if (cat.step == step.step_id)
                    {
                        category = cat;
                        break;
                    }
                }
                if (category==null)
                    return true;

                DateTime stepDate = startDate.AddDays(category.days);
                if (Utils.CompareDates(stepDate, step.date) != 0)
                    return true;
            }

            return false;
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

        static public bool UpdateStepRecordDate(Transaction tx, int stepID, DateTime date)
        {
            TransactionStep txStep = TransactionStep.GetRecordForTx(tx.id, stepID);
            return txStep.UpdateDate(date);
        }

        // Starting from the yeast, recreate the dates 
        static public bool UpdateAndAdjustStepsRecordDate(Transaction tx, int stepID, DateTime date, out string datesAdjusted)
        {
            List<Wine_Category> categories;
            Wine_Category.GetAllRecordsForID(tx.wine_category_id.ToString(), out categories);
            datesAdjusted = null;

            foreach (Wine_Category category in categories)
            {
                if (category.step < stepID)
                    continue; // we don't care about passed steps
                else 
                {
                    TransactionStep txStep = TransactionStep.GetRecordForTx(tx.id, category.step);
                    if (txStep.done > 0)
                        continue;

                    DateTime newCurrentDate = date;

                    newCurrentDate = newCurrentDate.AddDays(category.days);

                        // **Not anymore**
                        // do not change date if the new date is before the current one.
                        //if (newCurrentDate < txStep.date)
                        //    continue;
                    if (Utils.CompareDates(txStep.date, newCurrentDate) == 0)
                        continue;

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

        public void GetTransactionStepJSONRecords(HttpContext context, List<TransactionStep> steps, List<Transaction> allTx)
        {
            int nbRows = steps.Count;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("total");
                jsonWriter.WriteValue(nbRows);
                jsonWriter.WritePropertyName("page");
                jsonWriter.WriteValue(1);
                jsonWriter.WritePropertyName("records");
                jsonWriter.WriteValue(nbRows);
                jsonWriter.WritePropertyName("rows");
                jsonWriter.WriteStartArray();

                int iterNb = 0;
                foreach (TransactionStep step in steps)
                {
                    Transaction tx = null;
                    foreach(Transaction tx2 in allTx)
                    {
                        if (tx2.id == step.transaction_id)
                        {
                            tx = tx2;
                            break;
                        }
                    }

                    if (tx == null)
                        continue;

                    //Customer customer = Customer.GetRecordByID(tx.client_id.ToString());

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("id");
                    jsonWriter.WriteValue(iterNb.ToString());
                    jsonWriter.WritePropertyName("cell");
                    jsonWriter.WriteStartArray();

                    jsonWriter.WriteValue(step.id.ToString());
                    jsonWriter.WriteValue(step.transaction_id.ToString());
                    jsonWriter.WriteValue(step.date.ToString());
                    jsonWriter.WriteValue(step.StepName);
                    jsonWriter.WriteValue(tx.Symbol);
                    jsonWriter.WriteValue(tx.BrandName);
                    jsonWriter.WriteValue(tx.TypeName);
                    jsonWriter.WriteValue(tx.CustomerName);
                    jsonWriter.WriteValue(tx.CustomerTelephone);
                    jsonWriter.WriteValue(tx.location);
                    jsonWriter.WriteValue(step.done);

                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                    iterNb++;
                }

                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject();
                string retString = sb.ToString();
                context.Response.Write(retString);
            }
        }

        public bool SetTransactionStepToDone(List<string> ids, bool undo=false)
        {
            string sqlQuery = "UPDATE " + c_dbTransactionStepName + " SET done=";

            bool res = false;
            bool first =true;

            if (undo)
                sqlQuery += "0 WHERE ";
            else
                sqlQuery += "1 WHERE ";

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

        public bool SetLocation(List<string> ids, string location)
        {
            string sqlQuery = "UPDATE " + c_dbTransactionName + " SET location=" + location;
            bool res = false;
            bool first = true;
            foreach (string id in ids)
            {
                if (first)
                { 
                    first = false;
                    sqlQuery += " WHERE id=" + id;
                }
                else
                    sqlQuery += " OR id=" + id;
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

        static public bool IsAllStepsDoneForTx(int txid)
        {
            Transaction tx = Transaction.GetRecord(txid);
            if (tx == null)
                return false;

            int nbStepDone = 0;
            int nbStepTotal = 0;
            return tx.AreAllStepDone(out nbStepDone, out nbStepTotal);
        }

        static public bool SetTransactionToDone(List<string> ids, bool undo = false)
        {
            string sqlQuery = "UPDATE " + c_dbTransactionName + " SET done=";
            bool res = false;
            bool first = true;

            if (undo)
                sqlQuery += "0 WHERE ";
            else
                sqlQuery += "1 WHERE ";

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
                // Cannot show page 
                Utils.MessageBox(null, "** Error **\\nCannot complete these transactions because they still have steps not completed.");
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
                        res = command.ExecuteNonQuery() == 1;
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
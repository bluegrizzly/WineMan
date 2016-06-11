using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WineMan.Core;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WineMan.Work
{
    /// <summary>
    /// Summary description for WorkToCompleteHandler
    /// </summary>
    public class WorkToCompleteHandler : IHttpHandler
    {
        TransactionsHelper m_TransactionHelper = new TransactionsHelper();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string showtx = context.Request.QueryString["showtx"];
            string operation = context.Request.QueryString["operation"];

            if (showtx == "true")
                ProcessTransactions(context, operation);
            else
                ProcessTransactionSteps(context, operation);
        }

        private void ProcessTransactions(HttpContext context, string operation)
        {
            if (operation != null && operation == "settodone")
            {
                bool undo = false;
                string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                List<string> data = JsonConvert.DeserializeObject<List<string>>(jsonString);
                if (data.Count == 1)
                {
                    Transaction tx = Transaction.GetRecord(data[0]);
                    if (tx.done)
                        undo = true;
                }
                TransactionsHelper.SetTransactionToDone(data, undo);
            }
            else if (operation != null && operation == "setlocation")
            {
                string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                List<string> data = JsonConvert.DeserializeObject<List<string>>(jsonString);

                string location = context.Request.QueryString["location"];
                m_TransactionHelper.SetLocation(data, location);
            }

            string showReadyOnly = context.Request.QueryString["showreadyonly"];
            string dateStr = context.Request.QueryString["date"];
            DateTime date = DateTime.Now;
            if (dateStr == "")
                date = DateTime.MinValue;
            else
                DateTime.TryParse(dateStr, out date);

            string dateStrEnd = context.Request.QueryString["dateend"];
            DateTime dateEnd = DateTime.Now;
            if (dateStrEnd == "")
                dateEnd = DateTime.MaxValue;
            else
            {
                if (DateTime.TryParse(dateStrEnd, out dateEnd))
                    dateEnd = dateEnd.AddDays(1); // add a day so the end date is included
            }
            
            EShow showFilter = EShow.Show_All;
            if (context.Request.QueryString["showdone"] == "0")
                showFilter = EShow.Show_NotDone;
            else if (context.Request.QueryString["showdone"] == "1")
                showFilter = EShow.Show_Done;
            else if (context.Request.QueryString["showdone"] == "2")
                showFilter = EShow.Show_All;

            string txID = context.Request.QueryString["txid"];
            string customer = context.Request.QueryString["customer"];
            string dateKind = context.Request.QueryString["datekind"];
            Transaction.DateKind edateKind=Transaction.DateKind.Unknown;
            if (dateKind == "Creation")
                edateKind = Transaction.DateKind.Creation;
            else if (dateKind == "Bottling")
                edateKind = Transaction.DateKind.Bottling;

            // Check for specific transaction
            List<Transaction> transactions = new List<Transaction>();
            int txIDnb=-1;
            if (txID != null && txID.Length > 0 && Int32.TryParse(txID, out txIDnb))
            {
                Transaction tx = Transaction.GetRecord(txIDnb);
                if (tx != null)
                {
                    transactions.Add(tx);
                    m_TransactionHelper.GetTransactionJSONRecords(context, transactions, true);
                    return;
                }
            }
            
            // Get all tx
            transactions = Transaction.GetAllRecords(showFilter, Transaction.FilterTypes.All,customer,date, dateEnd, showReadyOnly == "true", edateKind);

            // Get the JSON
            m_TransactionHelper.GetTransactionJSONRecords(context, transactions, true);
        }

        private void ProcessTransactionSteps(HttpContext context, string operation)
        {
            if (operation != null && operation == "settodone")
            {
                bool undo = false;
                string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                List<string> data = JsonConvert.DeserializeObject<List<string>>(jsonString);
                if (data.Count == 1)
                {
                    TransactionStep step = TransactionStep.GetRecord(data[0]);
                    if (step.done > 0)
                        undo = true;
                }
                m_TransactionHelper.SetTransactionStepToDone(data, undo);
            }
            
            {
                NameValueCollection forms = context.Request.Form;
                string strOperation = forms.Get("oper");

                if (strOperation == null)
                {
                    string dateStr = context.Request.QueryString["date"];
                    DateTime date = DateTime.Now;
                    if (dateStr == "")
                        date = DateTime.MinValue;
                    else
                        DateTime.TryParse(dateStr, out date);

                    string dateStrEnd = context.Request.QueryString["dateend"];
                    DateTime dateEnd = DateTime.Now;
                    if (dateStrEnd == "")
                        dateEnd = DateTime.MaxValue;
                    else
                    {
                        DateTime.TryParse(dateStrEnd, out dateEnd);
                    }

                    EShow showdone = EShow.Show_All;
                    if (context.Request.QueryString["showdone"] == "0")
                        showdone = EShow.Show_NotDone;
                    else if (context.Request.QueryString["showdone"] == "1")
                        showdone = EShow.Show_Done;
                    else if (context.Request.QueryString["showdone"] == "2")
                        showdone = EShow.Show_All;

                    int filterStep = -1;
                    Int32.TryParse(context.Request.QueryString["filterstep"], out filterStep);
                    if (filterStep == 0)
                        filterStep = - 1;
                    string txID = context.Request.QueryString["txid"];
                    string customer = context.Request.QueryString["customer"];

                    // Get all transaction not completed.
                    List<TransactionStep> steps = m_TransactionHelper.GetAllStepsOfThisDay(date, dateEnd, false, showdone, filterStep, txID, customer);

                    // Sort by steps
                    steps.Sort((x, y) => x.date.CompareTo(y.date));
                    steps.Sort((x, y) => x.step_id.CompareTo(y.step_id));

                    m_TransactionHelper.GetTransactionStepJSONRecords(context, steps );
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
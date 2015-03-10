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
                m_TransactionHelper.SetTransactionToDone(data, undo);
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
            
            EShow showFilter = context.Request.QueryString["showdone"] == "true" ? EShow.Show_All : EShow.Show_NotDone;

            List<Transaction> transactions = Transaction.GetRecords(date, dateEnd, showFilter, showReadyOnly=="true");
            m_TransactionHelper.GetTransactionToCompleteJSONRecords(context, transactions);
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
                        if (DateTime.TryParse(dateStrEnd, out dateEnd))
                            dateEnd = dateEnd.AddDays(1); // add a day so the end date is included
                    }

                    EShow showdone = context.Request.QueryString["showdone"] == "true" ? EShow.Show_All : EShow.Show_NotDone;
                    int filterStep = -1;
                    Int32.TryParse(context.Request.QueryString["filterstep"], out filterStep);
                    if (filterStep == 0)
                        filterStep = - 1;
                    string txID = context.Request.QueryString["txid"];

                    // Get all transaction not completed.
                    List<TransactionStep> steps = m_TransactionHelper.GetAllStepsOfThisDay(date, dateEnd, false, showdone, filterStep, txID);

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
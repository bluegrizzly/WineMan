using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WineMan.Core;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WineMan.Transactions
{
    public class TransactionHandler : IHttpHandler
    {
        const string dbName = "transactions";
        HandlerHelper m_Helper = new HandlerHelper();
        TransactionsHelper m_TransactionHelper = new TransactionsHelper();
        public void ProcessRequest(HttpContext context)
        {
            string operation = context.Request.QueryString["operation"];

            if (operation != null)
            {
                if (operation == "editrow")
                {
                    string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                    string txID = JsonConvert.DeserializeObject<string>(jsonString);
                    if (txID != null)
                        context.Response.Write(@"""AddTransaction.aspx?txid=" + txID.ToString() + @"""");
                }
            }
            else
            {
                bool showCompleted = context.Request.QueryString["showcompleted"] == "true";
                string transactionIDStr = context.Request.QueryString["filtertxid"];
                int filterInt;
                bool parsed = Int32.TryParse(context.Request.QueryString["filterdate"], out filterInt);

                int txID=-1;
                if (Int32.TryParse(transactionIDStr, out txID))
                {
                    List<Transaction> allTx = new List<Transaction>();
                    Transaction tx = Transaction.GetRecord(txID);
                    if (tx != null && tx.id >= 0)
                    {
                        allTx.Add(tx);
                        m_TransactionHelper.GetTransactionJSONRecords(context, allTx);
                    }
                }
                else
                {
                    List<Transaction> allTx = Transaction.GetAllRecords(showCompleted, (Transaction.FilterTypes)filterInt, 
                                                                        context.Request.QueryString["filtercustomer"]);
                    m_TransactionHelper.GetTransactionJSONRecords(context, allTx);
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
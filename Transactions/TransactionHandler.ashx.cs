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
            NameValueCollection forms = context.Request.Form;
            string strOperation = forms.Get("oper");

            if (operation != null)
            {
                if (operation == "editrow")
                {
                    string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                    List<string> data;
                    if (jsonString.Contains(","))
                        data = JsonConvert.DeserializeObject<List<string>>(jsonString);
                    else
                    {
                        data = new List<string>();
                        data.Add(jsonString);
                    }

                    // first items are the selection,
                    // separated by a * after show all items
                    if (data.Count >= 1)
                    {
                        string txID = data[0];
                        string allIds=""; 
                        if (data.Count > 2 && data[1]=="*")
                        {
                            for (int i = 2; i < data.Count; i++)
                            {
                                allIds += data[i] ;
                                if (i < data.Count - 1)
                                    allIds += ",";
                            }
                        }
                        
                        string url = Utils.ResolveServerUrl("/Transactions/AddTransaction.aspx", false);
                        url += "?txid=" + txID.ToString();
                        
                        if (allIds.Length > 0)
                            url += "&alltxids=" + allIds;

                        context.Response.Write(@"""" + url + @"""");
                    }
                }
            }
            else if (strOperation == "del")
            {
                string strOut = string.Empty;
                string idToDelete = forms.Get("EmpId").ToString();

                // TODO : Confirm to user

                //1. delete all transaction steps related to this transaction
                TransactionStep.DeleteTxRecords(idToDelete);

                //2. delete the transaction
                if (DBAccess.DeleteRecord(context, dbName, idToDelete))
                    strOut = "Record successfully deleted";
                else
                    strOut = "Error in removing record";
                context.Response.Write(strOut);
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
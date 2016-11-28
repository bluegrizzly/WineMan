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
    public class TransactionHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
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
                            context.Session["AllTxIds"] = allIds;
                        else
                            context.Session.Remove("AllTxIds");

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
                EShow show = EShow.Show_All;
                if (context.Request.QueryString["showcompleted"] == "0")
                    show = EShow.Show_NotDone;
                else if (context.Request.QueryString["showcompleted"] == "1")
                    show = EShow.Show_Done;
                else if (context.Request.QueryString["showcompleted"] == "2")
                    show = EShow.Show_All;

                string transactionIDStr = context.Request.QueryString["filtertxid"];
                int filterInt;
                bool parsed = Int32.TryParse(context.Request.QueryString["filterdate"], out filterInt);

                int txID=-1;
                Int32.TryParse(transactionIDStr, out txID);

                string filtercustomers = context.Request.QueryString["filtercustomer"];
                List<Transaction> allTx = Transaction.GetAllRecords(show, (Transaction.FilterTypes)filterInt, filtercustomers, DateTime.MinValue, DateTime.MaxValue, false, Transaction.DateKind.Unknown, txID);
                m_TransactionHelper.GetTransactionJSONRecords(context, allTx, false);
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
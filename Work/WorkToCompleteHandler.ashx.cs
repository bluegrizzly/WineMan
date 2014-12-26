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

            string operation = context.Request.QueryString["operation"];

            if (operation != null && operation == "settodone")
            {
                string jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
                List<string> data = JsonConvert.DeserializeObject<List<string>>(jsonString);
                m_TransactionHelper.SetTransactionStepToDone(data);
            }
            
            {
                NameValueCollection forms = context.Request.Form;
                string strOperation = forms.Get("oper");

                if (strOperation == null)
                {
                    string dateStr = context.Request.QueryString["date"];
                    DateTime date = DateTime.Now;
                    DateTime.TryParse(dateStr, out date);

                    string dateStrEnd = context.Request.QueryString["dateend"];
                    DateTime dateEnd;
                    if (!DateTime.TryParse(dateStrEnd, out dateEnd))
                        dateEnd = date;
                    dateEnd = dateEnd.AddDays(1);

                    bool showlate = context.Request.QueryString["showlate"] == "true"?true : false;
                    EShow showdone = context.Request.QueryString["showdone"] == "true" ? EShow.Show_All : EShow.Show_NotDone;

                    // Get all transaction not completed.
                    List<TransactionStep> steps = m_TransactionHelper.GetAllStepsOfThisDay(date, dateEnd, showlate, showdone);
                    m_TransactionHelper.GetTransactionStepJSONRecords(context, steps);
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
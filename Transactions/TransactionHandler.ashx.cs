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
                m_Helper.ProcessRequest(context, dbName);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WineMan.Core;
using System.Collections.Specialized;

namespace WineMan.Customers
{
    /// <summary>
    /// Summary description for CustomersHandler
    /// </summary>
    public class CustomersHandler : IHttpHandler
    {
        const string dbName = "customers";
        HandlerHelper m_Helper = new HandlerHelper();
        public void ProcessRequest(HttpContext context)
        {
            m_Helper.ProcessRequest(context, dbName, false, true);
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
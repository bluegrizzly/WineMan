using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WineMan.Core;
using System.Collections.Specialized;

namespace WineMan.Admin
{
    /// <summary>
    /// Summary description for HolidaysHandler
    /// </summary>
    public class AdminHandler : IHttpHandler
    {

        HandlerHelper m_Helper = new HandlerHelper();
        public void ProcessRequest(HttpContext context)
        {
            string dbName = context.Request.QueryString["db"];
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
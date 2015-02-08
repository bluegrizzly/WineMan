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
            NameValueCollection forms = context.Request.Form;
            string strOperation = forms.Get("oper");

            string filterCustomer = context.Request.QueryString["filtercustomer"];
            if (strOperation == null && filterCustomer != null)
            {
                string sqlQuery=null;

                //try to see if it is teh complete name
                string[] arrayString = filterCustomer.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (arrayString.Count() == 2)
                {
                    sqlQuery = "SELECT DISTINCT * FROM " + dbName + " WHERE first_name LIKE '%" + arrayString[0] + "%'" +
                        " AND last_name LIKE '%" + arrayString[1] + "%'";
                }
                else
                {
                    sqlQuery = "SELECT DISTINCT * FROM " + dbName + " WHERE first_name LIKE '%" + filterCustomer + "%'" +
                        " OR last_name LIKE '%" + filterCustomer + "%'" +
                        " OR id LIKE '" + filterCustomer + "'" +
                        " OR telephone LIKE '" + filterCustomer + "'" +
                        " ORDER BY last_name";
                }

                DBAccess.GetJSONRecords(context, dbName, sqlQuery);
            }
            else
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
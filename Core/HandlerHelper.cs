using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WineMan.Core;
using System.Collections.Specialized;

namespace WineMan
{
    public class HandlerHelper 
    {
        public void ProcessRequest(HttpContext context, string dbName)
        {
            NameValueCollection forms = context.Request.Form;
            string strOperation = forms.Get("oper");

            string strResponse = string.Empty;
            if (strOperation == null)
            {
                DBAccess.GetJSONRecords(context, dbName);
            }
            else if (strOperation == "del")
            {
                string strOut = string.Empty;

                string idToDelete = forms.Get("EmpId").ToString();
                if (DBAccess.DeleteRecord(context, dbName, idToDelete))
                    strOut = "Record successfully deleted";
                else
                    strOut = "Error in removing record";
                context.Response.Write(strOut);
            }
            else if (strOperation == "add")
            {
                string strOut = string.Empty;
                if (DBAccess.AddRecord(context, dbName, forms))
                    strOut = "Record successfully added";
                else
                    strOut = "Error in adding record";
                context.Response.Write(strOut);
            }
            else if (strOperation == "edit")
            {
                string idToEdit = forms.Get("EmpId").ToString();
                string strOut = string.Empty;
                if (DBAccess.EditRecord(context, dbName, forms, idToEdit))
                    strOut = "Record successfully updated";
                else
                    strOut = "Error in updating record";
                context.Response.Write(strOut);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }
    }
}
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
                // special cases to get the name instead of ID for certain fields
                string sqlQuery=null;
                if (dbName == "wine_categories")
                    sqlQuery = "SELECT wine_categories.id, wine_categories.name, wine_categories.cost, steps.name AS StepName, wine_categories.days, wine_categories.symbol FROM wine_categories INNER JOIN steps ON wine_categories.step = steps.id ORDER BY wine_categories.id";
                else if (dbName == "wine_types")
                    sqlQuery = "SELECT wine_types.id, wine_types.name, wine_brands.name AS BrandName, wine_categories.name AS CategoryName, wine_types.active FROM wine_types INNER JOIN wine_brands ON wine_types.brand_id = wine_brands.id INNER JOIN wine_categories ON wine_types.category_id = wine_categories.id ORDER BY wine_types.id";

                DBAccess.GetJSONRecords(context, dbName, sqlQuery);
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
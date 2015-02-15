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
        public void ProcessRequest(HttpContext context, string dbName, bool explicitSetID=false, bool processDoublon=false)
        {
            NameValueCollection forms = context.Request.Form;
            string strOperation = forms.Get("oper");

            string strResponse = string.Empty;
            if (strOperation == null)
            {
                // special cases to get the name instead of ID for certain fields
                string sqlQuery=null;
                if (dbName == Wine_Brand.c_dbName)
                    sqlQuery = "SELECT * FROM " + dbName  + " ORDER BY name";
                else if (dbName == Wine_Category.c_dbName )
                    sqlQuery = "SELECT wine_categories.id, wine_categories.name, wine_categories.cost, steps.name AS StepName, wine_categories.days, wine_categories.symbol FROM wine_categories INNER JOIN steps ON wine_categories.step = steps.id ORDER BY wine_categories.name";
                else if (dbName == Wine_Type.c_dbName)
                    sqlQuery = "SELECT wine_types.id, wine_types.name, wine_brands.name AS BrandName, wine_categories.name AS CategoryName, wine_types.active FROM wine_types INNER JOIN wine_brands ON wine_types.brand_id = wine_brands.id INNER JOIN wine_categories ON wine_types.category_id = wine_categories.id ORDER BY wine_types.name";
                else if (dbName == Customer.c_dbName)
                    sqlQuery = "SELECT * FROM " + dbName + " ORDER BY last_name";

                DBAccess.GetJSONRecords(context, dbName, sqlQuery);
            }
            else if (strOperation == "del")
            {
                string strOut = string.Empty;
                string idToDelete = forms.Get("EmpId").ToString();

                if (dbName == Customer.c_dbName)
                {
                    List<Transaction> allTx;
                    if (Transaction.GetAllRecordsForCustomer(idToDelete, out allTx) > 0 )
                    {
                        string numbers="";
                        foreach (Transaction tx in allTx)
                            numbers += tx.id.ToString() + ",";

                        strOut = "Error: Customer is referenced in transactions ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }
                }
                else if (dbName == Wine_Category.c_dbName)
                {
                    List<Wine_Type> allTypes = Wine_Type.GetAllRecordsWithCategory(idToDelete);
                    if (allTypes.Count > 0)
                    {
                        string numbers = "";
                        foreach (Wine_Type type in allTypes)
                            numbers += type.id.ToString() + ",";

                        strOut = "Error: Wine Category is referenced in Wine Types ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }

                    List<Transaction> allTx = Transaction.GetAllRecordsWithWineCategory(idToDelete);
                    if( allTx.Count > 0)
                    {
                        string numbers = "";
                        foreach (Transaction tx in allTx)
                            numbers += tx.id.ToString() + ",";

                        strOut = "Error: Wine Category is referenced in Transactions ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }
                }
                else if (dbName == Wine_Brand.c_dbName)
                {
                    List<Wine_Type> allTypes = Wine_Type.GetAllRecordsWithBrand(idToDelete);
                    if (allTypes.Count > 0)
                    {
                        string numbers = "";
                        foreach (Wine_Type type in allTypes)
                            numbers += type.id.ToString() + ",";

                        strOut = "Error: Wine Brand is referenced in Wine Types ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }
                    List<Transaction> allTx = Transaction.GetAllRecordsWithWineBrand(idToDelete);
                    if (allTx.Count > 0)
                    {
                        string numbers = "";
                        foreach (Transaction tx in allTx)
                            numbers += tx.id.ToString() + ",";

                        strOut = "Error: Wine Brand is referenced in Transactions ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }
                }
                else if (dbName == Wine_Type.c_dbName)
                {
                    List<Transaction> allTx = Transaction.GetAllRecordsWithWineType(idToDelete);
                    if (allTx.Count > 0)
                    {
                        string numbers = "";
                        foreach (Transaction tx in allTx)
                            numbers += tx.id.ToString() + ",";

                        strOut = "Error: Wine Type is referenced in Transactions ID: " + numbers;
                        context.Response.Write(strOut);
                        return;
                    }
                }

                if (DBAccess.DeleteRecord(context, dbName, idToDelete))
                    strOut = "Record successfully deleted";
                else
                    strOut = "Error in removing record";
                context.Response.Write(strOut);
            }
            else if (strOperation == "add")
            {
                string strOut = string.Empty;

                if (processDoublon && DBAccess.IsRecordExistWithSameName(context, dbName, forms, explicitSetID))
                {
                    strOut = "Error: An entry with same name exists";
                    context.Response.Write(strOut);
                    return;
                }

                if (DBAccess.AddRecord(context, dbName, forms, explicitSetID))
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

namespace WineMan.Admin
{
    public partial class WineTypesManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        [WebMethod]
        public static string GetBrands()
        {
            string ret="<select>";
            for (int i = 0; i < 114; i++)
            {
                ret += @"<option value='" + i.ToString() + @"'>Volvo</option>";
            }
            ret += "</select>";
            return ret;

        }
        [WebMethod]
        public static string GetCategories()
        {
            //            page.SelectCustomer(name);
            string ret="";
            for (int i = 0; i < 201; i++)
            {
                ret += i.ToString() + ":val" + i.ToString() + ";";
            }    
            return ret;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan
{
    public partial class BaseReport : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UrlReferrer != null && ViewState["RefUrl"] == null)
                ViewState["RefUrl"] = Request.UrlReferrer.ToString();
        }

        protected void Button_Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewState["RefUrl"].ToString());
        }
    }
}
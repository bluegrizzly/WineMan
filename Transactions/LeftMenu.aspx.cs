using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Transactions
{
    public partial class LeftMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Edit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Transactions/AddTransaction.aspx");
        }
    }
}
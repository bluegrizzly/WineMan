using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Transactions
{
    public partial class Transactions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Button_ClearCustomer_Click(object sender, EventArgs e)
        {
            TextBox_CustomerSearch.Text = "";
        }

        protected void Button_ClearTxID_Click(object sender, EventArgs e)
        {
            TextBox_TxIDSearch.Text = "";
        }
    }
}
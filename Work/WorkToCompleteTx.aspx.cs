using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Work
{
    public partial class WorkToCompleteTx : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.ProcessPendingMessages(this);
            if (!IsPostBack)
            {
                RadioButtonList_Date.SelectedIndex = 1;

                // select today for the end date
                txtDateEnd.Text = DateTime.Now.ToString("MMM-dd-yyyy");
            }
        }

        protected void Button_ClearStart_Click(object sender, EventArgs e)
        {
            txtDateStart.Text = "";
        }

        protected void Button_ClearEnd_Click(object sender, EventArgs e)
        {
            txtDateEnd.Text = "";
        }

        protected void Button_ClearCustomer_Click(object sender, EventArgs e)
        {
            TextBox_Customer.Text = "";
        }

        protected void Button_ClearTxID_Click(object sender, EventArgs e)
        {
            TextBox_TxID.Text = "";
        }
    }
}
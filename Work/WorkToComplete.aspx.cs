using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

namespace WineMan.Work
{
    public partial class WorkToComplete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (txtDate.Text == "")
            //    txtDate.Text = DateTime.Now.ToString("yyyy-MM-d");
            //DateTime dd;
            //DateTime.TryParse(txtDate.Text, out dd);

            //Label_SelectedDate.Text = dd.ToString("MMM dd yyy");

            if (!IsPostBack)
            {
                DropDownList_FilterStep.Items.Clear();
                List<Step> steps = Step.GetAllRecords();
                DropDownList_FilterStep.Items.Add("All");
                foreach (Step step in steps)
                {
                    DropDownList_FilterStep.Items.Add(step.name);
                }


                // select today for the end date
                txtDateEnd.Text = DateTime.Now.ToString("MMM-dd-yyyy");
            }
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Work/WorkToComplete_Print.aspx?date=" + txtDate.Text + "&dateEnd=" + txtDateEnd.Text);
        }

        protected void Button_ClearTxID_Click(object sender, EventArgs e)
        {
            TextBox_TxID.Text = "";
        }
    }
}
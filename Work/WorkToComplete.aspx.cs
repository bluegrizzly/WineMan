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
        public string DateValue{ get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string aa = txtDate.Text;

            if (txtDate.Text == "")
                txtDate.Text = DateTime.Now.ToString("yyyy-MM-d");
            DateTime dd;
            DateTime.TryParse(txtDate.Text, out dd);

            Label_SelectedDate.Text = dd.ToString("MMM dd yyy");

            if (Request.QueryString["mode"] != "close")
            {
                //System.Web.UI.HtmlControls.HtmlButton button = Master.FindControl("setToDone") as System.Web.UI.HtmlControls.HtmlButton;
                //if (button!=null)
                //    button.Visible = false;
            }
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Work/WorkToComplete_Print.aspx?date=" + txtDate.Text + "&dateEnd=" + txtDateEnd.Text);
        }
    }
}
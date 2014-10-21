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
            Calendar_Date.SelectedDate = DateTime.Now;
            this.DateValue = DateTime.Now.ToString("d");
        }

        protected void Calendar_Date_SelectionChanged(object sender, EventArgs e)
        {
            txtDate.Text = Calendar_Date.SelectedDate.ToString("d");
            this.DateValue = DateTime.Now.ToString("d");
        }
    }
}
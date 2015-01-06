using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

namespace WineMan
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Calendar1.TodayDayStyle.BackColor = System.Drawing.Color.DeepSkyBlue;
         //   Calendar1.CssClass =
        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (Holiday.IsHoliday(e.Day.Date))
            {
                e.Cell.BackColor = System.Drawing.Color.LightGray;
                e.Day.IsSelectable = false;
            }
        }
    }
}

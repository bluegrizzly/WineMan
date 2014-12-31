using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WineMan;

namespace WineMan.Admin
{
    public partial class holidays : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Calendar_Holidays_DayRender(object sender, DayRenderEventArgs e)
        {
            if (Holiday.IsHoliday(e.Day.Date))
            {
                e.Cell.BackColor = System.Drawing.Color.LightGray;
                e.Day.IsSelectable = false;
            }
        }
    }
}
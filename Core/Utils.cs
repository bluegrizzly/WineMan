using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace WineMan
{
    public enum EShow
    {
        Show_All,
        Show_Done,
        Show_NotDone
    };

    public static class Utils
    {
        public static void MessageBox(this Page Page, String Message) { Page.ClientScript.RegisterStartupScript(Page.GetType(), "MessageBox", "<script language='javascript'>alert('" + Message + "');</script>"); }

    }
}
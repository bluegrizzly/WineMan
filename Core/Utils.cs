using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;

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
        public static void MessageBox(Page Page, String Message) 
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Message", "<script type='text/javascript'>window.onload = function() {alert('" + Message +"');return false;}</script>", false );
            // see http://www.tizag.com/javascriptT/javascriptconfirm.php   for a confirm button
        }

        public static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
                return null;

            // *** Absolute path - just return
            if (originalUrl.IndexOf("://") != -1)
                return originalUrl;

            // *** Fix up image path for ~ root app dir directory
            if (originalUrl.StartsWith("~"))
            {
                string newUrl = "";
                if (HttpContext.Current != null)
                    newUrl = HttpContext.Current.Request.ApplicationPath +
                          originalUrl.Substring(1).Replace("//", "/");
                else
                    // *** Not context: assume current directory is the base directory
                    throw new ArgumentException("Invalid URL: Relative URL not allowed.");

                // *** Just to be sure fix up any double slashes
                return newUrl;
            }

            return originalUrl;
        }

        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            // *** Is it already an absolute Url?
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            // *** Start by fixing up the Url an Application relative Url
            string newUrl = ResolveUrl(serverUrl);

            Uri originalUri = HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                     "://" + originalUri.Authority + newUrl;

            return newUrl;
        }

        // Returns:
        //     A signed number indicating the relative values of this instance and the value
        //     parameter.Value Description Less than zero This instance is earlier than
        //     value. Zero This instance is the same as value. Greater than zero This instance
        //     is later than value.
        public static int CompareDates(DateTime date1, DateTime date2)
        {
            if (date1.Year < date2.Year)
                return -1;
            else if (date1.Year > date2.Year)
                return 1;
            else if (date1.Month < date2.Month)
                return -1;
            else if (date1.Month > date2.Month)
                return 1;
            else if (date1.Day < date2.Day)
                return -1;
            else if (date1.Day > date2.Day)
                return 1;

            return 0;
        }
    }
}
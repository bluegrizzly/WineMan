using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
//using CSASPNETMessageBox;

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
            //MessageBox messageBox = new MessageBox();
            //messageBox.MessageTitle = "Information";
            //messageBox.MessageText = Message;
            //messageBox.Show(Page); 
            Page.Response.Write("<script type=text/javascript>alert('" + Message + "');</script>");

            //Page.ClientScript.RegisterStartupScript(Page.GetType(), "MessageBox", "<script type=text/javascript> alert('" + Message + "')</script>"); 
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
 

    }
}
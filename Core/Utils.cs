using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;
using System.Management;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

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
        public static string s_pendingMessage="";
        public static void MessageBox(Page page, String message) 
        {
            if (page == null)
            {
                Utils.s_pendingMessage = message;
                return;
            }

            ScriptManager.RegisterStartupScript(page, page.GetType(), "Message", "<script type='text/javascript'>window.onload = function() {alert('" + message +"');return false;}</script>", false );
            // see http://www.tizag.com/javascriptT/javascriptconfirm.php   for a confirm button
        }
        public static void ProcessPendingMessages(Page page)
        {
            if (s_pendingMessage != "")
            {
                MessageBox(page, s_pendingMessage);
                s_pendingMessage = "";
            }
        }

        public static bool MessageBoxQuestion(Page Page, String Message)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Message", "<script type='text/javascript'>window.onload = function() {var answer = confirm('" + Message + "');return answer;}</script>", false);
            return true;
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

        public static List<string> GetAllPrinters()
        {
            List<string> printers = new List<string>();

            System.Management.ManagementScope objMS = new System.Management.ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            System.Management.ManagementObjectCollection objMOC = objMOS.Get();

            foreach (ManagementObject Printers in objMOC)
            {
                string serverName = @"\\" + Printers["ServerName"] + @"\";
                if (Convert.ToBoolean(Printers["Local"]))       // LOCAL PRINTERS.
                {
                    printers.Add(Printers["Name"].ToString());
                }
                if (Convert.ToBoolean(Printers["Network"]))     // ALL NETWORK PRINTERS.
                {
                    printers.Add(Printers["Name"].ToString());
                }
            }
            return printers;
        }

        public static string FormatTelephone(string phoneNum)
        {
            string phoneFormat = "(###) ###-####";
            // First, remove everything except of numbers
            Regex regexObj = new Regex(@"[^\d]");
            phoneNum = regexObj.Replace(phoneNum, "");

            // Second, format numbers to phone string 
            if (phoneNum.Length > 0)
            {
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
            }

            return phoneNum;
        }

        public static void InitialSetup()
        {
            try
            {
                // 1. TABLE openhours 
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string sqlQuery = @"CREATE TABLE IF NOT EXISTS `openhours` (
                                        `id` INT NOT NULL AUTO_INCREMENT,
                                        `day` VARCHAR(45) NULL,
                                        `hour_start` INT NULL DEFAULT 10,
                                        `hour_end` INT NULL DEFAULT 18,
                                        PRIMARY KEY (`id`),
                                        UNIQUE INDEX `id_UNIQUE` (`id` ASC))";

                    con.Open();
                    int result = -1;
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        result = cmd.ExecuteNonQuery();
                    }

                    sqlQuery = @"SELECT COUNT(*) FROM openhours";
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                    {
                        result =  Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (result <= 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            DayOfWeek day = (System.DayOfWeek)i;

                            sqlQuery = @"INSERT INTO `openhours` (`day`, `hour_start`, `hour_end`) VALUES ('" + day.ToString() + @"', '9', '18')";
                            using (MySqlCommand cmd = new MySqlCommand(sqlQuery, con))
                            {
                                result = cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Assert(false, e.Message);
            }
        }
    }
}
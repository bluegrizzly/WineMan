using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace WineMan.Work
{
    public partial class DoBackup : System.Web.UI.Page
    {
        Settings m_Settings = new Settings();
        protected void Page_Load(object sender, EventArgs e)
        {
            Label_Path.Text = m_Settings.backup_path;
        }

        protected void Button_DoBackup_Click(object sender, EventArgs e)
        {

            string destination = m_Settings.backup_path;
            destination += "\\";
            destination += DateTime.Now.ToString("yyyy-MM-dd");
            destination += ".sql";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = con;
                            con.Open();
                            mb.ExportToFile(destination);
                            con.Close();

                            TextBox_Log.Text = "Backup successful to: " + destination;
                            TextBox_Log.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.Assert(false, exp.Message);
                TextBox_Log.Text = "Backup Failed to: " + destination;
                TextBox_Log.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void Button_ImportCustomers_Click(object sender, EventArgs e)
        {
            string appPath = Request.PhysicalApplicationPath;
            string saveDir = @"\Uploads\";
            if (FileUpload1.HasFile)
            {
                string savePath = appPath + saveDir +
                    Server.HtmlEncode(FileUpload1.FileName);
            }
        }
    }
}
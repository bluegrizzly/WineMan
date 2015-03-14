using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Admin
{
    public partial class Settings : System.Web.UI.Page
    {
        const string selectString = "Please select a default printer";
        private WineMan.Settings m_Settings = new WineMan.Settings();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //populate the dropdown
                bool isHavingDefault = (m_Settings.default_printer != null && m_Settings.default_printer != "");
                if (!isHavingDefault)
                    DropDownList_Printers.Items.Add(selectString);

                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    DropDownList_Printers.Items.Add(printer);
                }

                if (isHavingDefault)
                    DropDownList_Printers.SelectedValue = m_Settings.default_printer;

                // Backup path
                TextBox_BackupPath.Text = m_Settings.backup_path;

                // Autoprint
                CheckBox_AutoPrint.Checked = m_Settings.auto_print;
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            m_Settings.default_printer = DropDownList_Printers.SelectedValue;
            m_Settings.backup_path = TextBox_BackupPath.Text;
            //make sure there is double path 
            m_Settings.backup_path.Replace(@"\", @"\\");
            m_Settings.auto_print = CheckBox_AutoPrint.Checked;
            m_Settings.Save();
        }
    }
}
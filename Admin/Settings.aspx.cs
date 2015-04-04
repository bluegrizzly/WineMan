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

                bool isHavingDefaultReports = (m_Settings.default_printerreports != null && m_Settings.default_printerreports != "");
                if (!isHavingDefaultReports)
                    DropDownList_PrintersReports.Items.Add(selectString);


                List<string> printers = Utils.GetAllPrinters();
                foreach (string printer in printers)
                {
                    DropDownList_Printers.Items.Add(printer);
                    DropDownList_PrintersReports.Items.Add(printer);
                }

                if (isHavingDefault)
                    DropDownList_Printers.SelectedValue = m_Settings.default_printer;
                if (isHavingDefaultReports)
                    DropDownList_PrintersReports.SelectedValue = m_Settings.default_printerreports;

                // Backup path
                TextBox_BackupPath.Text = m_Settings.backup_path;

                // Autoprint
                CheckBox_AutoPrint.Checked = m_Settings.auto_print;

                TextBox_StartTXID.Text = m_Settings.transaction_starting_id.ToString();
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            m_Settings.default_printer = DropDownList_Printers.SelectedValue;
            m_Settings.default_printerreports = DropDownList_PrintersReports.SelectedValue;
            m_Settings.backup_path = TextBox_BackupPath.Text;
            m_Settings.auto_print = CheckBox_AutoPrint.Checked;

            if (!Int32.TryParse(TextBox_StartTXID.Text, out m_Settings.transaction_starting_id))
            {
                Utils.MessageBox(this, "** Error **\\nTransaction ID is not valid.");
                m_Settings.transaction_starting_id = 0;
            }

            if (m_Settings.Save())
                Utils.MessageBox(this, "** Success **\\nThe settings have been saved.");
            else
                Utils.MessageBox(this, "** Error **\\nThe settings was NOT saved.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;
using WineMan;

namespace WineMan.Work
{
    public partial class WorkToComplete_Print : System.Web.UI.Page
    {
        private DateTime m_DateStart = DateTime.Now;
        private DateTime m_DateEnd = DateTime.Now;
        private int m_txID = -1;
        private int m_StepId = -1;
        private string m_Customer="";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the data from page arguments
            string dateStr = Request.QueryString["date"];
            if (!DateTime.TryParse(dateStr, out m_DateStart))
            {
                m_DateStart = DateTime.MinValue;
            }
            dateStr = Request.QueryString["dateend"];
            if (!DateTime.TryParse(dateStr, out m_DateEnd))
            {
                m_DateEnd= DateTime.MaxValue;
            }
            if (!Int32.TryParse(Request.QueryString["txid"], out m_txID))
                m_txID = -1;
            if (!Int32.TryParse(Request.QueryString["step"], out m_StepId))
                m_StepId = -1;
            m_Customer = Request.QueryString["customer"];

            if (!IsPostBack)
            {
                FillData();
                if (Request.UrlReferrer != null)
                    ViewState["RefUrl"] = Request.UrlReferrer.ToString();
                else
                    Button_Back.Enabled = false;

                List<string> printers = Utils.GetAllPrinters();
                Settings setting = new Settings();
                foreach (string printer in printers)
                {
                    DropDownList_Printer.Items.Add(printer);
                    if (setting.default_printerreports == printer)
                    {
                        DropDownList_Printer.SelectedIndex = DropDownList_Printer.Items.Count - 1;
                    }
                }
            }
        }

        protected void Button_Show_Click(object sender, EventArgs e)
        {
            FillData();
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, false);
            //reportPrint.PrintWithDialog();
            if (DropDownList_Printer.Items.Count == 0)
            {
                Utils.MessageBox(this, "** Error **\nNo Printers");
            }
            else
            {
                reportPrint.SetPrinter(DropDownList_Printer.SelectedItem.Text);
                reportPrint.Print();
            }
        }

        private void FillData()
        {
            ReportViewer1.Reset();

            // Data Sources
            ReportDataSource rptSrcStepNames = new ReportDataSource("DataSetStepNames", GetStepNamesData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcStepNames);
            ReportDataSource rptSrcTx = new ReportDataSource("DataSetTx", GetTransactionData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);
            ReportDataSource rptSrcCustomer = new ReportDataSource("DataSetCustomer", GetCustomerData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcCustomer);
            ReportDataSource rptSrcstepA = new ReportDataSource("DataSetA", GetAggregation());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcstepA);
            
            // Report path
            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_WorkToComplete.rdlc";

            // Parameters
            ReportParameter param1 = new ReportParameter("ReportParameter_Date", m_DateEnd.ToString("dd MMM yyyy"));
            ReportViewer1.LocalReport.SetParameters(param1);

            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetAggregation()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                //DateTime oldDate = new DateTime(1, 1, 1);
                //string dateStr = oldDate.Year.ToString() + "-" + oldDate.Month.ToString() + "-" + oldDate.Day.ToString() + " %";
                //string dateStrEnd = m_DateStart.Year.ToString() + "-" + m_DateStart.Month.ToString() + "-" + m_DateStart.Day.ToString() + " %";

                string dateStr = m_DateStart.Year.ToString() + "-" + m_DateStart.Month.ToString() + "-" + m_DateStart.Day.ToString() + " %";
                string dateStrEnd = m_DateEnd.Year.ToString() + "-" + m_DateEnd.Month.ToString() + "-" + m_DateEnd.Day.ToString() + " %";

                string sqlQuery1 = "SELECT  transaction_step.id, transaction_step.transaction_id, transaction_step.step_id, transaction_step.`date`, transaction_step.done, transactions.id AS Expr1, " +
                                            "transactions.wine_brand_id, wine_brands.name, customers.first_name, customers.last_name, wine_types.name AS Expr2, customers.telephone" +
                                    " FROM transaction_step INNER JOIN" +
                                         " transactions ON transaction_step.transaction_id = transactions.id INNER JOIN" +
                                         " wine_brands ON transactions.wine_brand_id = wine_brands.id INNER JOIN" +
                                         " customers ON transactions.client_id = customers.id INNER JOIN" +
                                         " wine_types ON transactions.wine_type_id = wine_types.id" +
                                    " WHERE (date BETWEEN '" + dateStr + "'" + " AND '" + dateStrEnd + "') AND transaction_step.done=0";

                if (m_txID >= 0)
                    sqlQuery1 += " AND transaction_step.transaction_id=" + m_txID.ToString();
                if (m_StepId >= 0)
                    sqlQuery1 += " AND transaction_step.step_id=" + m_StepId.ToString();

                if (m_Customer != null && m_Customer.Length > 0)
                {
                    sqlQuery1 += " AND (customers.last_name LIKE '%" + m_Customer + "%' OR customers.first_name LIKE '%" + m_Customer + "%')";
                }

                sqlQuery1 += " ORDER BY step_id";

                MySqlDataAdapter adp = new MySqlDataAdapter(sqlQuery1, con);
                adp.Fill(dt);
            }
            return dt;

        }
        private DataTable GetStepNamesData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM steps", con);
                adp.Fill(dt);
            }
            return dt;
        }
        private DataTable GetTransactionData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM transactions", con);
                adp.Fill(dt);
            }
            return dt;
        }
        private DataTable GetCustomerData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM customers", con);
                adp.Fill(dt);
            }
            return dt;
        }

        protected void Button_Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(ViewState["RefUrl"].ToString());
        }
    }
}
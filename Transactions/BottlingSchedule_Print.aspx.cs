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

namespace WineMan.Transactions
{
    public partial class BottlingSchedule_Print : System.Web.UI.Page
    {
        private DateTime m_DateStart = DateTime.Now;
        private static System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the data from page arguments
            string dateStr = Request.QueryString["date"];
            if (!DateTime.TryParse(dateStr, out m_DateStart))
            {
                m_DateStart = DateTime.MinValue;
            }
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
            ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, true);
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
            ReportDataSource rptSrcAppointments = new ReportDataSource("DataSetTx", GetAppointments(0));
            ReportViewer1.LocalReport.DataSources.Add(rptSrcAppointments);
            ReportDataSource rptSrcAppointments2 = new ReportDataSource("DataSetTx_2", GetAppointments(1));
            ReportViewer1.LocalReport.DataSources.Add(rptSrcAppointments2);
            ReportDataSource rptSrcAppointments3 = new ReportDataSource("DataSetTx_3", GetAppointments(2));
            ReportViewer1.LocalReport.DataSources.Add(rptSrcAppointments3);

            ReportDataSource rptSrcCustomers = new ReportDataSource("DataSetCustomer", GetCustomers());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcCustomers);
            
            // Report path
            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_BottlingSchedule.rdlc";

            // Parameters
            ReportParameter param1 = new ReportParameter("ReportParameter_Date", m_DateStart.DayOfWeek.ToString() + " " + m_DateStart.ToString("dd MMM yyyy"));
            ReportViewer1.LocalReport.SetParameters(param1);

            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetAppointments(int stationNo)
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM transactions WHERE date_bottling LIKE '" + m_DateStart.ToString("yyyy-MM-dd") + "%'";
                sqlQuery += " AND  bottling_station='" + stationNo.ToString() + "'";


                string sqlQuery1 = "SELECT transactions.*, customers.first_name AS CUSTO_FirstName, customers.last_name AS CUSTO_LastName, customers.telephone AS CUSTO_Tel, wine_categories.symbol AS CATEGORY_Symbol FROM transactions INNER JOIN" +
                                    " customers ON transactions.client_id = customers.id INNER JOIN" +
                                    " wine_categories ON transactions.wine_category_id = wine_categories.id" +
                                    " WHERE date_bottling LIKE '" + m_DateStart.ToString("yyyy-MM-dd") + "%'" +
                                    " AND  bottling_station='" + stationNo.ToString() + "'";


                MySqlDataAdapter adp = new MySqlDataAdapter(sqlQuery1, con);
                adp.Fill(dt);
            }
            return dt;
        }

        private DataTable GetCustomers()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM customers";
                MySqlDataAdapter adp = new MySqlDataAdapter(sqlQuery, con);
                adp.Fill(dt);
            }
            return dt;
        }

        protected void Button_Back_Click(object sender, EventArgs e)
        {
            string date = m_DateStart.ToString("yyyy-MM-dd", m_Culture);
            Response.Redirect("~/Transactions/RendezVous.aspx?date=" + date);
        }
    }
}
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
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the data 
            string dateStr = Request.QueryString["date"];
            DateTime.TryParse(dateStr, out m_DateStart);

        }

        protected void Button_Show_Click(object sender, EventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportViewer1.Reset();

            // Data Sources
            ReportDataSource rptSrcTx = new ReportDataSource("DataSetStepYeast", GetStepYeast());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);
            ReportDataSource rptSrcStepNames = new ReportDataSource("DataSetStepNames", GetStepNamesData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcStepNames);

            // Report path
            ReportViewer1.LocalReport.ReportPath = "Reports/Report_WorkToComplete.rdlc";

            // Parameters
            ReportParameter param1 = new ReportParameter("ReportParameter_Date", m_DateStart.ToString("dd MMM yyyy"));
            ReportViewer1.LocalReport.SetParameters(param1);

            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetStepYeast()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                DateTime oldDate = new DateTime(1, 1, 1);
                string sqlQuery = TransactionStep.BuildSQLQuery(oldDate, m_DateStart, EShow.Show_NotDone);
                sqlQuery += " ORDER BY step_id";
                MySqlDataAdapter adp = new MySqlDataAdapter(sqlQuery, con);
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
    }
}
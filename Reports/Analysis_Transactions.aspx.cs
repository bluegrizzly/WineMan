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

namespace WineMan.Reports
{
    public partial class Analysis_Transactions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillData();
            }
        }
        private void FillData()
        {
            ReportViewer1.Reset();

            ReportDataSource rptSrcTx = new ReportDataSource("DataSet1", GetTransactionData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);

            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_Analysis_Tx.rdlc";

            ReportViewer1.LocalReport.Refresh();
        }
        private DataTable GetTransactionData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM transactions " , con);
                adp.Fill(dt);
            }
            return dt;
        }
    }
}
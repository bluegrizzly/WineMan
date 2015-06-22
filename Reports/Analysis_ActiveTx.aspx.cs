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
    public partial class Analysis_ActiveTx : System.Web.UI.Page
    {
        private int m_TotalTxCount = 0;
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

            ReportDataSource rptSrcTx = new ReportDataSource("DataSet_ActiveTx", GetTransactionData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);

            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_Analysis_ActiveTx.rdlc";

            ReportParameter param1 = new ReportParameter("ReportParameter_TotalTxCount", m_TotalTxCount.ToString());
            ReportViewer1.LocalReport.SetParameters(param1);

            ReportViewer1.LocalReport.Refresh();
        }
        private DataTable GetTransactionData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string sqlQuery1 = "SELECT transactions.*, wine_types.name AS WINETYPE_Name, wine_brands.name AS WINEBRAND_Name FROM transactions INNER JOIN" +
                                    " wine_types ON transactions.wine_type_id = wine_types.id INNER JOIN " +
                                    " wine_brands ON transactions.wine_brand_id = wine_brands.id " +
                                    " WHERE done=0";

                MySqlDataAdapter adp = new MySqlDataAdapter(sqlQuery1, con);
                adp.Fill(dt);

                m_TotalTxCount = dt.Rows.Count;
            }
            return dt;
        }
    }
}
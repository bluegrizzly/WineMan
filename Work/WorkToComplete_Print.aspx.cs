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
            if (!DateTime.TryParse(dateStr, out m_DateStart))
            {
                m_DateStart = DateTime.Now;
            }
            if (!IsPostBack)
            {
                FillData();
                if (Request.UrlReferrer != null)
                    ViewState["RefUrl"] = Request.UrlReferrer.ToString();
                else
                    Button_Back.Enabled = false;
            }
        }

        protected void Button_Show_Click(object sender, EventArgs e)
        {
            FillData();
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, false);
            reportPrint.Print();
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
            ReportViewer1.LocalReport.ReportPath = "Reports/Report_WorkToComplete.rdlc";

            // Parameters
            ReportParameter param1 = new ReportParameter("ReportParameter_Date", m_DateStart.ToString("dd MMM yyyy"));
            ReportViewer1.LocalReport.SetParameters(param1);

            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetAggregation()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                DateTime oldDate = new DateTime(1, 1, 1);

                string dateStr = oldDate.Year.ToString() + "-" + oldDate.Month.ToString() + "-" + oldDate.Day.ToString() + " %";
                string dateStrEnd = m_DateStart.Year.ToString() + "-" + m_DateStart.Month.ToString() + "-" + m_DateStart.Day.ToString() + " %";

                string sqlQuery1 = "SELECT  transaction_step.id, transaction_step.transaction_id, transaction_step.step_id, transaction_step.`date`, transaction_step.done, transactions.id AS Expr1, " +
                                            "transactions.wine_brand_id, wine_brands.name, customers.first_name, customers.last_name, wine_types.name AS Expr2, customers.telephone" +
                                    " FROM transaction_step INNER JOIN" +
                                         " transactions ON transaction_step.transaction_id = transactions.id INNER JOIN" +
                                         " wine_brands ON transactions.wine_brand_id = wine_brands.id INNER JOIN" +
                                         " customers ON transactions.client_id = customers.id INNER JOIN" +
                                         " wine_types ON transactions.wine_type_id = wine_types.id" +
                                    " WHERE (date BETWEEN '" + dateStr + "'" + " AND '" + dateStrEnd + "') AND transaction_step.done=0" +
                                    " ORDER BY step_id";

                //string sqlQuery = TransactionStep.BuildSQLQuery(oldDate, m_DateStart, EShow.Show_NotDone);
                //sqlQuery += " ORDER BY step_id";
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
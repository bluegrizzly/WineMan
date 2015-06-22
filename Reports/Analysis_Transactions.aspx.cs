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

            ReportDataSource rptSrcTx = new ReportDataSource("DataSet2", GetTransactionData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);

            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_Analysis_Tx.rdlc";

            ReportViewer1.LocalReport.Refresh();
        }
        private DataTable GetTransactionData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Week", Type.GetType("System.String"));
            dt.Columns.Add("NbOpen", Type.GetType("System.Int32"));

            string[] names = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            DateTime date = new DateTime(DateTime.Now.Year-1, DateTime.Now.Month+1, 1);
            for (int i = 0; i < 12; i++)
            {
                List<Transaction> allTx = Transaction.GetAllRecordsCreatedInMonth(date);

                DataRow row = dt.NewRow();
                row["Week"] = names[date.Month-1];
                row["NbOpen"] = allTx.Count;
                dt.Rows.Add(row);

                date = date.AddMonths(1);
            }
            return dt;
        }
    }
}
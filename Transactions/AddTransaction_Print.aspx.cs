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
    public partial class AddTransaction_Print : System.Web.UI.Page
    {
        Transaction m_tx;
        Settings m_Settings = new Settings();
        protected void Page_Load(object sender, EventArgs e)
        {
            int txID=-1;
            if (!IsPostBack)
            {
                ReportViewer1.AsyncRendering = false;
                if (Request.QueryString["Tx"] != null)
                {
                    Label_Tx.Text = Request.QueryString["Tx"];
                    bool parsed = Int32.TryParse(Request.QueryString["Tx"], out txID);
                    System.Diagnostics.Debug.Assert(parsed);
                    m_tx = Transaction.GetRecord(txID);
                    FillData();

                    if (m_Settings.auto_print)
                    {
                        if (m_Settings.default_printer == "")
                            Utils.MessageBox(this, "** Error **\\nNo Default Printer[Transactions] selected.\\nPlease go in the Admin->Settings and set a default printer.");
                        else
                        {
                            ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, true);
                            reportPrint.SetPrinter(m_Settings.default_printer);
                            reportPrint.Print();
                            Response.Redirect(Request.UrlReferrer.ToString());
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }
                if (Request.UrlReferrer != null)
                    ViewState["RefUrl"] = Request.UrlReferrer.ToString();
                else
                    Button_Back.Enabled = false;
            }
            else
            {
                bool parsed = Int32.TryParse(Label_Tx.Text, out txID);
                System.Diagnostics.Debug.Assert(parsed);
                m_tx = Transaction.GetRecord(txID);
            }
        }

        protected void ButtonShow_Click(object sender, EventArgs e)
        {
            FillData();
        }

        protected void ButtonPrint_Click(object sender, EventArgs e)
        {
            if (m_Settings.default_printer == "")
                Utils.MessageBox(this, "** Error **\\nNo Default Printer selected.\\nPlease go in the Admin->Settings and set a default printer.");
            else
            {
                ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, true);
                reportPrint.SetPrinter(m_Settings.default_printer);
                reportPrint.Print();
            }
        }

        private void FillData()
        {
            ReportViewer1.Reset();

            ReportDataSource rptSrcCustomer = new ReportDataSource("DataSetCustomer", GetCustomerData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcCustomer);
            ReportDataSource rptSrcTx = new ReportDataSource("DataSetTx", GetTransactionData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcTx);
            ReportDataSource rptSrcCategory = new ReportDataSource("DataSetCategory", GetWineCategoryData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcCategory);
            ReportDataSource rptSrcSteps = new ReportDataSource("DataSetSteps", GetStepsData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcSteps);
            ReportDataSource rptSrcStepNames = new ReportDataSource("DataSetStepNames", GetStepNamesData());
            ReportViewer1.LocalReport.DataSources.Add(rptSrcStepNames);

            ReportViewer1.LocalReport.ReportPath = "Bin/Reports/Report_Transaction.rdlc";
            
            // Parameters
            string labelColor = "White";
            Wine_Category category = Wine_Category.GetRecordByID(m_tx.wine_category_id.ToString());
            Wine_Brand brand = Wine_Brand.GetRecordByID(m_tx.wine_brand_id.ToString());
            Wine_Type type = Wine_Type.GetRecordByID(m_tx.wine_type_id.ToString());

            if (m_tx.product_code > 0 && m_tx.product_code != Int16.MaxValue)
            {
                Product_Code code = Product_Code.GetRecordByID(m_tx.product_code);
                labelColor = code.color;
            }

            ReportParameter paramColor = new ReportParameter("ReportParameter_Color", labelColor);
            ReportViewer1.LocalReport.SetParameters(paramColor);

            ReportParameter param1 = new ReportParameter("ReportParameter_WineBrand", brand.name);
            ReportViewer1.LocalReport.SetParameters(param1);
            ReportParameter param2 = new ReportParameter("ReportParameter_WineType", type.name);
            ReportViewer1.LocalReport.SetParameters(param2);
            ReportParameter param3 = new ReportParameter("ReportParameter_WineCategory", category.name);
            ReportViewer1.LocalReport.SetParameters(param3);
            ReportParameter param4 = new ReportParameter("ReportParameter_ProductCode", m_tx.product_code.ToString());
            ReportViewer1.LocalReport.SetParameters(param4);
            
            ReportViewer1.LocalReport.Refresh();
        }

        private DataTable GetCustomerData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM customers WHERE id=" + m_tx.client_id.ToString(), con);
                adp.Fill(dt);
            }
            return dt;
        }
        private DataTable GetTransactionData()
        {
            WineMan.DataSets.DataSetTx.transactionsDataTable dt = new WineMan.DataSets.DataSetTx.transactionsDataTable();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM transactions WHERE id=" + m_tx.id.ToString(), con);
                adp.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        // Bar Code
                        System.Drawing.Image img = GenCode128.Code128Rendering.MakeBarcodeImage("00" + m_tx.id.ToString(), int.Parse("2"), true);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        row["bar_code2"] = ms.ToArray();
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.Assert(false, e.Message);
                    }
                }
            }
            return dt;
        }
        private DataTable GetWineCategoryData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM wine_categories WHERE id=" + m_tx.wine_category_id.ToString(), con);
                adp.Fill(dt);
            }
            return dt;
        }
        private DataTable GetStepsData()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM transaction_step WHERE transaction_id=" + m_tx.id.ToString(), con);
                adp.Fill(dt);
            }

            // Fill with blank to 
            for (int i = dt.Rows.Count; i < 10; i++)
                dt.Rows.Add(dt.NewRow());
            
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

        protected void Button_Back_Click(object sender, EventArgs e)
        {
            if (ViewState["RefUrl"] == null)
                return;

            string url = ViewState["RefUrl"].ToString();
            if (url.Contains("?"))
                url = url.Remove(url.IndexOf("?"));

            url += ("?txid=" + m_tx.id.ToString());

            Response.Redirect(url);
        }

    }
}
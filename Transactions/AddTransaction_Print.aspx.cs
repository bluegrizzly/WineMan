﻿using System;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            int txID=-1;
            if (!IsPostBack)
            {
                if (Request.QueryString["Tx"] != null)
                {
                    Label_Tx.Text = Request.QueryString["Tx"];
                    bool parsed = Int32.TryParse(Request.QueryString["Tx"], out txID);
                    System.Diagnostics.Debug.Assert(parsed);
                    m_tx = Transaction.GetRecord(txID);
                    FillData();
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }
            }
            else
            {
                bool parsed = Int32.TryParse(Label_Tx.Text, out txID);
                System.Diagnostics.Debug.Assert(parsed);
                m_tx = Transaction.GetRecord(txID);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            FillData();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            ReportPrintDocument reportPrint = new ReportPrintDocument(ReportViewer1.LocalReport, true);
            
            reportPrint.Print();

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

            string labelColor = "Pink";
            Wine_Category category = Wine_Category.GetRecordByID(m_tx.wine_category_id.ToString());
            if (category.symbol.Contains("R"))
                labelColor = "Red";
            else if (category.symbol.Contains("W"))
                labelColor = "Gold";

            ReportParameter param = new ReportParameter("ReportParameter_Color", labelColor);

            ReportViewer1.LocalReport.ReportPath = "Reports/Report_Transaction.rdlc";
            ReportViewer1.LocalReport.SetParameters(param);
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
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["winemanConnectionString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT * FROM transactions WHERE id=" + m_tx.id.ToString(), con);
                adp.Fill(dt);
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
    }
}
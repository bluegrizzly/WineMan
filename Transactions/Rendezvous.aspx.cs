﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Transactions
{
    public partial class Rendezvous : System.Web.UI.Page
    {
        Settings m_Settings = new Settings();
        List< List<RadioButton> > m_RadioButtons = new List< List<RadioButton> >();
        private int m_Station=-1; //zero based
        private int m_Hour = -1;
        private string m_Name;
        List<Transaction> m_Transactions;
        List<Holiday> m_Holidays;
        private static System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");

        const int c_NbColumnPerStation = 6;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // if the page is not comming from the add transaction, we need to clear the session data 
                // this means that we are not required to pass data to the add transaction page.
                if (Request.QueryString["FromAddTx"] != "true")
                    Session.Clear();

                if (Request.UrlReferrer != null)
                    ViewState["RefUrl"] = Request.UrlReferrer.ToString();

                // set the calendar date
                if (Session["rendezvous"] != null)
                    Calendar_RDV.SelectedDate = (DateTime)Session["rendezvous"];
                else
                    Calendar_RDV.SelectedDate = Calendar_RDV.TodaysDate;

                Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);
            }
            m_Name = Request.QueryString["customer"];

            if (Session["rendezvous_hour"] != null)
            {
                string aa = (string)Session["rendezvous_hour"];
                bool parsed = Int32.TryParse((string)Session["rendezvous_hour"], out m_Hour);
                System.Diagnostics.Debug.Assert(parsed);
            }

            if (Session["rendezvous_station"] != null)
            {
                bool parsed = Int32.TryParse((string)Session["rendezvous_station"], out m_Station); //zero based
                System.Diagnostics.Debug.Assert(parsed);
            }

            // Populate all radio buttons
            for (int j = 0; j < m_Settings.NbStations; ++j)
            {
                List<RadioButton> row = new List<RadioButton>();
                m_RadioButtons.Add(row);
                for (int i = m_Settings.MinStationHour; i < m_Settings.MaxStationHour; ++i)
                {
                    RadioButton radioButton = new RadioButton();
                    radioButton.GroupName = "spot";
                    //radioButton.CheckedChanged += new EventHandler(this.RadioButton_CheckedChanged);
                    radioButton.PreRender += new EventHandler(this.RadioButton_PreRender);
                    radioButton.AutoPostBack = true;
                    m_RadioButtons[j].Add(radioButton);

                    if (m_Hour == i && m_Station == j)
                    {
                        // Need to select the line that was passed from the add transaction
                        radioButton.Checked = true;
                    }
                }
            }

            Label_Date.Text = Calendar_RDV.SelectedDate.ToString("D", m_Culture);

            CreateTable(Calendar_RDV.SelectedDate);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Button_Select.Visible = (Request.QueryString["FromAddTx"] == "true");

            // Get all transactions for this day
            m_Transactions = Transaction.GetRecords(Calendar_RDV.SelectedDate);
            m_Holidays = Holiday.GetAllRecords();

            // Populate the rendez-vous
            for (int i = m_Settings.MinStationHour; i < m_Settings.MaxStationHour; ++i)
            {
                for (int j = 0; j < m_Settings.NbStations; ++j)
                {
                    TableCell cell = Table_Stations.Rows[2 + i - m_Settings.MinStationHour].Cells[j * c_NbColumnPerStation];
                    Transaction foundTx = null;
                    foreach (Transaction tx in m_Transactions)
                    {
                        if (tx.date_bottling.Hour == i && tx.bottling_station == j)
                        {
                            foundTx = tx;
                            break;
                        }
                    }

                    if (foundTx!=null)
                    {
                        GetCell(CellKind.Gray, cell);
                        m_RadioButtons[j][i - m_Settings.MinStationHour].Visible = false;

                        // Hour
                        TableCell cellHour = Table_Stations.Rows[2 + i - m_Settings.MinStationHour].Cells[(j * c_NbColumnPerStation) + 1];
                        GetCell(CellKind.Gray, cellHour);

                        // tx
                        TableCell celltx = Table_Stations.Rows[2 + i - m_Settings.MinStationHour].Cells[(j * c_NbColumnPerStation) + 2];
                        celltx.Text = "<a href=AddTransaction.aspx?txid=" + foundTx.id.ToString() + ">" + foundTx.id.ToString() + "</a>";
                        GetCell(CellKind.Gray, celltx);

                        // customer name
                        TableCell cellName = Table_Stations.Rows[2 + i - m_Settings.MinStationHour].Cells[(j * c_NbColumnPerStation) + 3];
                        GetCell(CellKind.Gray,cellName);
                        Customer cus = Customer.GetRecordByID(foundTx.client_id.ToString());
                        if (cus != null)
                            cellName.Text = cus.FirstName + " " + cus.LastName;
                        
                        // customer phone
                        TableCell cellPhone = Table_Stations.Rows[2 + i - m_Settings.MinStationHour].Cells[(j * c_NbColumnPerStation) + 4];
                        GetCell(CellKind.Gray, cellPhone);
                        if (cus != null)
                            cellPhone.Text = cus.Telephone;
                    }
                }
            }
        }

        private void SelectLine(TableRow row, int columnIndex)
        {
            RadioButton button = row.Cells[columnIndex].Controls[0] as RadioButton;

            if (button != null)
            {
                button.Checked = true;
                for (int i = 0; i < c_NbColumnPerStation - 1; ++i)
                {
                    row.Cells[columnIndex + i].BackColor = System.Drawing.Color.Aquamarine;
                    if (i == 3)
                        row.Cells[columnIndex + i].Text = m_Name;
                }
            }
        }

        private void RadioButton_PreRender(Object sender, EventArgs e)
        {
            // Select the line
            RadioButton radioButton = sender as RadioButton;

            if (radioButton.Checked)
            {
                foreach (TableRow row in Table_Stations.Rows)
                {
                    foreach (TableCell cell in row.Cells)
                    {
                        if (cell.Controls.Count > 0 && radioButton == cell.Controls[0])
                        {
                            SelectLine(row, row.Cells.GetCellIndex(cell));
                            return;
                        }
                    }
                }
            }
        }

        private void SaveRendezVousValues()
        {
            // Set the information for communication with previous page
            int selectedHour = -1;

            for (int j = 0; j < m_Settings.NbStations; ++j)
            {
                for (int i = m_Settings.MinStationHour; i < m_Settings.MaxStationHour; ++i)
                {
                    if (m_RadioButtons[j][i - m_Settings.MinStationHour].Checked)
                    {
                        selectedHour = i;
                        m_Station = j;
                        break;
                    }
                }
            }

            Session["rendezvous"] = Calendar_RDV.SelectedDate;
            Session["rendezvous_hour"] = selectedHour.ToString();
            Session["rendezvous_station"] = (m_Station).ToString();
        }

        enum CellKind { Normal, Black, Gray, Title, Selected };
        
        private TableCell GetCell(CellKind cellKind, TableCell cell=null)
        {
            TableCell tCell = (cell==null)?new TableCell():cell;
            
            switch (cellKind)
            {
                case CellKind.Black:
                    tCell.BackColor = System.Drawing.Color.Black;
                    break;
                case CellKind.Gray:
                    tCell.BackColor = System.Drawing.Color.LightGray;
                    break;
                case CellKind.Title:
                    tCell.ForeColor = System.Drawing.Color.White;
                    tCell.BackColor = System.Drawing.Color.RoyalBlue;
                    tCell.Font.Size = 14;
                    break;
                case CellKind.Selected:
                    tCell.BackColor = System.Drawing.Color.Aquamarine;
                    break;
            }
            return tCell;
        }


        private void CreateTable(DateTime selectedDate)
        {
            Table_Stations.Font.Size = 8;
            TableRow tRowStations = new TableRow();
            Table_Stations.Rows.Add(tRowStations);
            TableRow tRowTitle = new TableRow();
            Table_Stations.Rows.Add(tRowTitle);

            for (int i = 0; i < m_Settings.NbStations; ++i)
            {
                TableCell tCell = GetCell(CellKind.Title);
                tCell.HorizontalAlign = HorizontalAlign.Center;
                tCell.Text = "Station " + (i).ToString();
                tCell.ColumnSpan = 5;
                tRowStations.Cells.Add(tCell);
                tRowStations.Cells.Add(GetCell(CellKind.Black));

                tCell = new TableCell();
                tCell.Text = "Select";
                tCell.ForeColor = System.Drawing.Color.Black;
                tCell.BackColor = System.Drawing.Color.LightGray;
                tRowTitle.Cells.Add(tCell);
                tCell = new TableCell();
                tCell.Text = "Hour";
                tCell.ForeColor = System.Drawing.Color.Black;
                tCell.BackColor = System.Drawing.Color.LightGray;
                tCell.Font.Name = "Arial";
                tCell.Font.Size = 12;
                tRowTitle.Cells.Add(tCell);
                tCell = new TableCell();
                tCell.Text = "TX";
                tCell.ForeColor = System.Drawing.Color.Black;
                tCell.BackColor = System.Drawing.Color.LightGray;
                tCell.Font.Name = "Arial";
                tCell.Font.Size = 12;
                tRowTitle.Cells.Add(tCell);
                tCell = new TableCell();
                tCell.Text = "Name";
                tCell.Font.Name = "Arial";
                tCell.Font.Size = 12;
                tCell.ForeColor = System.Drawing.Color.Black;
                tCell.BackColor = System.Drawing.Color.LightGray;
                tRowTitle.Cells.Add(tCell);
                tCell = new TableCell();
                tCell.Text = "Phone";
                tCell.Font.Name = "Arial";
                tCell.Font.Size = 12;
                tCell.ForeColor = System.Drawing.Color.Black;
                tCell.BackColor = System.Drawing.Color.LightGray;
                tRowTitle.Cells.Add(tCell);
                tRowTitle.Cells.Add(GetCell(CellKind.Black));
            }

            for (int i = m_Settings.MinStationHour; i < m_Settings.MaxStationHour; ++i)
            {
                TableRow tRow = new TableRow();
                Table_Stations.Rows.Add(tRow);
                
                for (int j = 0; j < m_Settings.NbStations; ++j)
                {
                    TableCell tCell = GetCell(CellKind.Normal);

                    // select
                    tCell.Controls.Add(m_RadioButtons[j][i - m_Settings.MinStationHour]);
                    tRow.Cells.Add(tCell);

                    // hour
                    tCell = GetCell(CellKind.Normal);
                    tCell.Text = i.ToString() + "h00";
                    tRow.Cells.Add(tCell);
                    // tx
                    tCell = GetCell(CellKind.Normal);
                    tRow.Cells.Add(tCell);
                    // Name
                    tCell = GetCell(CellKind.Normal);
                    tRow.Cells.Add(tCell);
                    // phone
                    tCell = GetCell(CellKind.Normal);
                    tRow.Cells.Add(tCell);
                    tRow.Cells.Add(GetCell(CellKind.Black));
                }
            }
        }

        protected void Button_Select_Click(object sender, EventArgs e)
        {
            if (ViewState["RefUrl"] == null)
                return;

            SaveRendezVousValues(); // pass it to the next page

            string refUrl = (string)ViewState["RefUrl"];
            
            // Return a flag to know that we started from the AddTransaction page.
            if (refUrl.Contains("AddTransaction"))
            {
                refUrl += "?FromAdd=true";
            }

            if (refUrl != null)
                Response.Redirect(refUrl);
        }
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            if (ViewState["RefUrl"] == null)
                return;

            string refUrl = (string)ViewState["RefUrl"];

            // Return a flag to know that we started from the AddTransaction page.
            if (refUrl.Contains("AddTransaction"))
            {
                refUrl += "?FromAdd=true";
            }

            if (refUrl != null)
                Response.Redirect(refUrl);
        }

        protected void Calendar_RDV_SelectionChanged(object sender, EventArgs e)
        {
            Label_Date.Text = Calendar_RDV.SelectedDate.ToString("D", m_Culture);
        }

        protected void Calendar_RDV_DayRender(object sender, DayRenderEventArgs e)
        {
            foreach(Holiday holiday in m_Holidays)
            {
                if (e.Day.Date.Year == holiday.date.Year &&
                    e.Day.Date.Month == holiday.date.Month &&
                    e.Day.Date.Day == holiday.date.Day)
                {
                    e.Cell.BackColor = System.Drawing.Color.LightGray;
                    e.Day.IsSelectable = false;
                }
            }
        }

    }
}

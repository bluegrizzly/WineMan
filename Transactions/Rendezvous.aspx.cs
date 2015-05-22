using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Transactions
{
    public class RendezVousTableRow
    {
        public TableRow Row;
        public Dictionary<int, RendezVousData> RdvData = new Dictionary<int, RendezVousData>();  // The key is the bottling station ID

        static public int GetRowKey(int hour, int minutes)
        {
            return hour *100+minutes;
        }
        static public int GetHourFromKey(int key)
        {
            return key / 100;
        }
        static public int GetMinFromKey(int key)
        {
            return key % 100;
        }
    }

    public class RendezVousData
    {
        public int Station;
        public int Hour;
        public int Min;
        public RadioButton RadioButton;
        public int GetRowKey()
        {
            return RendezVousTableRow.GetRowKey(Hour,Min);
        }
    }

    public partial class Rendezvous : System.Web.UI.Page
    {
        Settings m_Settings = new Settings();
        private int m_Station=-1; //zero based
        private int m_Hour = -1;
        private string m_CustomerName;
        List<Transaction> m_Transactions;
        List<Holiday> m_Holidays;
        public Dictionary<int, RendezVousTableRow> m_TableRows = new Dictionary<int, RendezVousTableRow>();  // The key is the HOUR*100+MIN
        private static System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");

        const int c_NbColumnPerStation = 6;

        int m_TxID = -1;
        bool m_FromAddTx = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            m_FromAddTx = Request.QueryString["FromAddTx"] == "true";
            if (Request.QueryString["txid"] != null)
                Int32.TryParse(Request.QueryString["txid"], out m_TxID);

            if (!IsPostBack)
            {
                // set the calendar date
                if (Session["rendezvous"] != null)
                    Calendar_RDV.SelectedDate = (DateTime)Session["rendezvous"];
                else
                    Calendar_RDV.SelectedDate = Calendar_RDV.TodaysDate;

                // if the page is not coming from the add transaction, we need to clear the session data 
                // this means that we are not required to pass data to the add transaction page.
                if (!m_FromAddTx)
                {
                    Session.Clear();
                }

                // Make sure the date is not a holiday
                while (Holiday.IsHoliday(Calendar_RDV.SelectedDate))
                {
                    Calendar_RDV.SelectedDate = Calendar_RDV.SelectedDate.AddDays(1);
                }

                Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);
            }

            m_CustomerName = Request.QueryString["customer"];

            Button_Select.Enabled = false; // will turn on when selecying a line

            if (Session["rendezvous_hour"] != null)
            {
                string time = (string)Session["rendezvous_hour"];

                if (time.Contains("h"))
                    m_Hour = RendezVousTableRow.GetRowKey(DateHelper.GetRendezVousHour(time), DateHelper.GetRendezVousMin(time));
                else
                {
                    bool parsed = Int32.TryParse(time, out m_Hour);
                    System.Diagnostics.Debug.Assert(parsed);
                }
            }

            if (Session["rendezvous_station"] != null)
            {
                bool parsed = Int32.TryParse((string)Session["rendezvous_station"], out m_Station); //zero based
                System.Diagnostics.Debug.Assert(parsed);
            }

            // New row to add (from Add button)
            if (Request.QueryString["NewHourRow"] != null)
            {
                int hourKey =0;
                bool parsed = Int32.TryParse((string)Request.QueryString["NewHourRow"], out hourKey);
                System.Diagnostics.Debug.Assert(parsed);
                int hour = RendezVousTableRow.GetHourFromKey(hourKey); ;
                int min = RendezVousTableRow.GetMinFromKey(hourKey); ;
                m_Hour = hourKey;
                m_Station = 0;
                AddTableRow(hour, min);
            }
            SetupAddButtonUrl();

            // Populate all data

            int minHour;
            int maxHour;
            OpenHours.GetDayOpenHour(Calendar_RDV.SelectedDate.DayOfWeek, out minHour, out maxHour);
            for (int j = minHour; j < maxHour; ++j)
            {
                int nbInTheHour = 60 / m_Settings.hour_intervale;
                for (int k = 0; k < nbInTheHour; ++k)
                {
                    int hour = j;
                    int min = k * 60 / (k + 1);
                    AddTableRow(hour, min);
                }
            }

            // Get all transactions for this day
            m_Transactions = Transaction.GetRecords(Calendar_RDV.SelectedDate);
            m_Holidays = Holiday.GetAllRecords();

            // add the special hours from all transactions 
            foreach (Transaction tx in m_Transactions)
            {
                if (tx.date_bottling.Minute != 0)
                {
                    AddTableRow(tx.date_bottling.Hour, tx.date_bottling.Minute);
                }
            }

            Label_Date.Text = Calendar_RDV.SelectedDate.ToString("D", m_Culture);

            CreateTable(Calendar_RDV.SelectedDate);
        }

        protected void AddTableRow(int hour, int min)
        {
            int rowKey = RendezVousTableRow.GetRowKey(hour, min);
            if (m_TableRows.ContainsKey(rowKey))
                return;

            RendezVousTableRow row = new RendezVousTableRow();
            row.Row = new TableRow();

            m_TableRows.Add(rowKey, row);

            for (int station = 0; station < m_Settings.NbStations; ++station)
            {
                RendezVousData data = new RendezVousData();
                data.Hour = hour;
                data.Min = min;
                data.Station = station;
                data.RadioButton = new RadioButton();
                data.RadioButton.GroupName = "spot";
                data.RadioButton.PreRender += new EventHandler(this.RadioButton_PreRender);
                data.RadioButton.AutoPostBack = true;
                if (m_Hour == RendezVousTableRow.GetRowKey(hour, min) && m_Station == station)
                {
                    // Need to select the line that was passed from the add transaction
                    data.RadioButton.Checked = true;
                }

                row.RdvData.Add(station, data);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Button_Select.Visible = m_FromAddTx;
            Button_Print.Visible = !m_FromAddTx;
            Button_Cancel.Visible = m_FromAddTx;

            foreach (KeyValuePair<int, RendezVousTableRow> row in m_TableRows)
            {
                foreach (KeyValuePair<int, RendezVousData> rdvData in row.Value.RdvData)
                {
                    TableCell cell = row.Value.Row.Cells[rdvData.Key * c_NbColumnPerStation];
                    Transaction foundTx = null;
                    int hour = RendezVousTableRow.GetHourFromKey(row.Key);
                    int min = RendezVousTableRow.GetMinFromKey(row.Key);

                    foreach (Transaction tx in m_Transactions)
                    {
                        if (tx.date_bottling.Hour == hour && tx.bottling_station == rdvData.Key && tx.date_bottling.Minute == min)
                        {
                            foundTx = tx;
                            break;
                        }
                    }

                    if (foundTx!=null)
                    {
                        GetCell(CellKind.Gray, cell);
                        rdvData.Value.RadioButton.Visible = (m_TxID == foundTx.id);

                        // Hour
                        TableCell cellHour = row.Value.Row.Cells[(rdvData.Key * c_NbColumnPerStation) + 1];
                        GetCell(CellKind.Gray, cellHour);

                        // tx
                        TableCell celltx = row.Value.Row.Cells[(rdvData.Key * c_NbColumnPerStation) + 2];
                        celltx.Text = "<a href=AddTransaction.aspx?txid=" + foundTx.id.ToString() + ">" + foundTx.id.ToString() + "</a>";
                        GetCell(CellKind.Gray, celltx);

                        // customer name
                        TableCell cellName = row.Value.Row.Cells[(rdvData.Key * c_NbColumnPerStation) + 3];
                        GetCell(CellKind.Gray,cellName);
                        Customer cus = Customer.GetRecordByID(foundTx.client_id.ToString());
                        if (cus != null)
                            cellName.Text = cus.first_name + " " + cus.last_name;
                        
                        // customer phone
                        TableCell cellPhone = row.Value.Row.Cells[(rdvData.Key * c_NbColumnPerStation) + 4];
                        GetCell(CellKind.Gray, cellPhone);
                        if (cus != null)
                            cellPhone.Text = cus.telephone;
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
                    row.Cells[columnIndex + i].BackColor = System.Drawing.Color.SkyBlue;
                    if (i == 3)
                        row.Cells[columnIndex + i].Text = m_CustomerName;
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

                            Button_Select.Enabled = true;
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
            int selectedStation = -1;

            foreach (KeyValuePair<int, RendezVousTableRow> row in m_TableRows)
            {
                foreach (KeyValuePair<int, RendezVousData> rdvData in row.Value.RdvData)
                {
                    if (rdvData.Value.RadioButton.Checked)
                    {
                        selectedHour = row.Key;
                        selectedStation = rdvData.Key;
                        break;
                    }
                }
            }

            if (selectedHour >= 0)
            {
                Session["rendezvous"] = Calendar_RDV.SelectedDate;
                int hour = RendezVousTableRow.GetHourFromKey(selectedHour);
                int min = RendezVousTableRow.GetMinFromKey(selectedHour);
                Session["rendezvous_hour"] = GetHourMinName(hour) + "h" + GetHourMinName(min); 
                Session["rendezvous_station"] = selectedStation .ToString();

                // Save transaction
                if (m_TxID >=0)
                {
                    Transaction tx = Transaction.GetRecord(m_TxID);
                    tx.date_bottling = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, Calendar_RDV.SelectedDate.Day);
                    tx.date_bottling = tx.date_bottling.AddHours(hour);
                    tx.date_bottling = tx.date_bottling.AddMinutes(min);
                    tx.bottling_station = selectedStation;

                    if (!tx.ModifyRecord())
                    {
                        Utils.MessageBox(this, "** Error **\\nFailed to modify a transaction.");
                    }
                }
            }
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
            
            foreach (KeyValuePair<int, RendezVousTableRow> row in m_TableRows.OrderBy(i => i.Key))
            {
                TableRow tRow = row.Value.Row;
                Table_Stations.Rows.Add(tRow);

                foreach (KeyValuePair<int, RendezVousData> rdvData in row.Value.RdvData)
                {
                    TableCell tCell = GetCell(CellKind.Normal);

                    // select
                    tCell.Controls.Add(rdvData.Value.RadioButton);
                    tRow.Cells.Add(tCell);

                    int hour = RendezVousTableRow.GetHourFromKey(row.Key);
                    int min = RendezVousTableRow.GetMinFromKey(row.Key);
                    // hour
                    tCell = GetCell(CellKind.Normal);
                    tCell.Text = GetHourMinName(hour) + "h" + GetHourMinName(min);
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

        protected string GetHourMinName(int hourormin)
        {
            string ret ="";
            if (hourormin.ToString().Length == 1)
                ret += "0";
            ret += hourormin.ToString();
            return ret;
        }

        protected string GetRefUrl(string basePath)
        {
            string refUrl = basePath;

            // Return a flag to know that we started from the AddTransaction page.
            if (refUrl.Contains("AddTransaction"))
            {
                if (!refUrl.Contains("?"))
                    refUrl += "?FromAdd=true";
                else
                    refUrl += "&FromAdd=true";

                if (m_TxID >= 0 && !refUrl.Contains("txid="))
                    refUrl += "&txid=" + m_TxID.ToString();
            }
            return refUrl;
        }

        protected void Button_Select_Click(object sender, EventArgs e)
        {
            if (!m_FromAddTx)
                return;

            SaveRendezVousValues(); // pass it to the next page

            ReturnToTransactionPage();
        }

        protected void ReturnToTransactionPage()
        {
            if (!m_FromAddTx)
                return;

            string refUrl = GetRefUrl("/Transactions/AddTransaction.aspx");

            if (refUrl != null)
                Response.Redirect(refUrl);
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            ReturnToTransactionPage();
        }

        protected void Calendar_RDV_SelectionChanged(object sender, EventArgs e)
        {
            if (m_FromAddTx && Session["wineready_date"] != null) 
            {
                // Warning: Make sure the date is not before the minimum date

                DateTime minDate = (DateTime)Session["wineready_date"];
                if (Calendar_RDV.SelectedDate < minDate)
                {
                    Utils.MessageBox(this, "** Warning **\\nThis appointment date is going to be BEFORE the wine is ready.\\nWine ready: " + minDate.ToString("D", m_Culture));
                }
            }
            //Label_Date.Text = Calendar_RDV.SelectedDate.ToString("D", m_Culture);
            Session["rendezvous"] = Calendar_RDV.SelectedDate;

            Response.Redirect(GetRefUrl(Request.RawUrl));
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

        protected void SetupAddButtonUrl()
        {
            int hour = 0;
            int min = 0;
            bool parsed = Int32.TryParse(DropDownList_ManualHour.SelectedValue, out hour);
            System.Diagnostics.Debug.Assert(parsed);
            parsed = Int32.TryParse(DropDownList_ManualMin.SelectedValue, out min);
            System.Diagnostics.Debug.Assert(parsed);

            int hourKey = RendezVousTableRow.GetRowKey(hour, min);
            if (!m_TableRows.ContainsKey(hourKey))
            {
                string url = "~/Transactions/Rendezvous.aspx?FromAddTx=";
                if (m_FromAddTx)
                    url += "true";
                else
                    url += "false";
                if (m_TxID >= 0)
                    url += "&txid=" + m_TxID.ToString();

                url += "&customer=" + m_CustomerName + "&NewHourRow=" + hourKey.ToString();
                Button_AddHour_S1.PostBackUrl = url;
            }
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            string date = Calendar_RDV.SelectedDate.ToString("yyyy-MM-dd", m_Culture);
            Response.Redirect("~/Transactions/RendezVous_Print.aspx?date=" + date);
        }
    }
}

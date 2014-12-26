using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Transactions
{
    public partial class RendezVous_Print : System.Web.UI.Page
    {
        private TableCell CreateNewCell(string text, TableRow tRow)
        {
            TableCell tCell = new TableCell();
            tCell.Text = text;
            tCell.BorderWidth = 1;
            tRow.Cells.Add(tCell);
            return tCell;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TransactionsHelper txHelper = new TransactionsHelper();

            // Get the data 
            string dateStr = Request.QueryString["date"];
            DateTime dateStart = DateTime.Now;
            DateTime.TryParse(dateStr, out dateStart);

            List<Transaction> allTx = Transaction.GetRecords(dateStart);

            // Print
            //Table_Content.BorderWidth = 1;
            Label1.Text = dateStart.ToString("ddd MMM d yyyy");

            // Sort by stations
            var sort = from s in allTx
                       orderby s.bottling_station
                       select s;

            int station = -1;
            foreach (Transaction txx in sort)
            {
                if (txx.bottling_station != station)
                {
                    if (station != -1)
                    {
                        TableRow tRowDummy = new TableRow();
                        Table_Content.Rows.Add(tRowDummy);
                    }

                    station = txx.bottling_station;
                    TableRow tRowTitle = new TableRow();
                    TableCell tCell = new TableCell();
                    tCell.HorizontalAlign = HorizontalAlign.Left;
                    tCell.BorderWidth = 2;
                    tCell.BackColor = System.Drawing.Color.LightGray;
                    tCell.Text = "Station " + station.ToString();
                    tCell.ForeColor = System.Drawing.Color.Black;
                    tRowTitle.Cells.Add(tCell);
                    Table_Content.Rows.Add(tRowTitle);
                }

                Customer customer = Customer.GetRecordByID(txx.client_id.ToString());
                Wine_Category category = Wine_Category.GetRecordByID(txx.wine_category_id.ToString());
                {
                    // transaction ID
                    TableRow tRow = new TableRow();
                    tRow.BorderWidth = 1;

                    //tCell.BorderWidth = 1;
                    CreateNewCell(txx.id.ToString(), tRow);

                    // Hour
                    CreateNewCell(txx.date_bottling.ToShortTimeString(), tRow);

                    // client 
                    CreateNewCell(customer.GetFullName(), tRow);

                    // client phone
                    CreateNewCell(customer.telephone, tRow);

                    // color
                    CreateNewCell(category.symbol, tRow);
                    
                    //TODO
                    // localisation
                    CreateNewCell("1?", tRow);

                    //TODO
                    // group
                    TableCell tCell = CreateNewCell("Group?", tRow);

                    // done
                    tCell = CreateNewCell("Group?", tRow);
                    CheckBox checkbox = new CheckBox();
                    checkbox.Checked = txx.done;
                    tCell.Controls.Add(checkbox);

                    Table_Content.Rows.Add(tRow);
                }
            }
        }
    }
}
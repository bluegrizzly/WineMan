using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WineMan.Work
{
    public partial class WorkdToComplete_Print : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TransactionsHelper txHelper = new TransactionsHelper();
            
            // Get the data 
            string dateStr = Request.QueryString["date"];
            DateTime dateStart = DateTime.Now;
            DateTime.TryParse(dateStr, out dateStart);

            string dateStrEnd = Request.QueryString["dateend"];
            DateTime dateEnd;
            if (!DateTime.TryParse(dateStrEnd, out dateEnd))
                dateEnd = dateStart;
            dateEnd = dateEnd.AddDays(1);

            List<TransactionStep> allTxSteps = TransactionStep.GetRecords(dateStart, dateEnd, EShow.Show_NotDone);
            List<TransactionStep> stepsLate;
            DateTime oldDate = new DateTime(1, 1, 1);
            stepsLate = TransactionStep.GetRecords(oldDate, dateStart, EShow.Show_NotDone);
            allTxSteps.AddRange(stepsLate);

            var sort = from s in allTxSteps
                        orderby s.step_id
                        select s;

            // Print
            //Table_Content.BorderWidth = 1;
            Label1.Text = dateStart.ToString("dd MMM yyyy");

            int stepKindID = -1;
            foreach (TransactionStep step in sort)
            {
                if (step.step_id != stepKindID)
                {
                    stepKindID = step.step_id;
                    TableRow tRowTitle = new TableRow();
                    TableCell tCell = new TableCell();
                    tCell.HorizontalAlign = HorizontalAlign.Left;
                    tCell.Text = txHelper.GetStepName(step.step_id).ToUpper();
                    tCell.ForeColor = System.Drawing.Color.Black;
                    tRowTitle.Cells.Add(tCell);
                    Table_Content.Rows.Add(tRowTitle);
                }

                Transaction tx = Transaction.GetRecord(step.transaction_id);
                Customer customer = Customer.GetRecordByID(tx.client_id.ToString());
                {
                    // transaction ID
                    TableRow tRow = new TableRow();
                    tRow.BorderWidth = 1;
                    TableCell tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = step.transaction_id.ToString();
                    tRow.Cells.Add(tCell);

                    // date
                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = step.date.ToString("dd/mm/yyyy");
                    tRow.Cells.Add(tCell);

                    // Brand
                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = txHelper.GetBrandName(tx.wine_brand_id);
                    tRow.Cells.Add(tCell);

                    // Type
                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = txHelper.GetTypeName(tx.wine_type_id);
                    tRow.Cells.Add(tCell);

                    // client name
                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = customer.GetFullName();
                    tRow.Cells.Add(tCell);
                    
                    // client phone
                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Text = customer.telephone;
                    tRow.Cells.Add(tCell);

                    tCell = new TableCell();
                    tCell.BorderWidth = 1;
                    tCell.Controls.Add(new CheckBox());
                    tRow.Cells.Add(tCell);

                    Table_Content.Rows.Add(tRow);
                }
            }
        }
    }
}
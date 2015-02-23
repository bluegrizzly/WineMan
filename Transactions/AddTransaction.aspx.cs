using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using WineMan;

namespace WineMan.Transactions
{
    public partial class AddTransaction : System.Web.UI.Page
    {
        private System.Drawing.Color m_DisabledColor;
        private System.Drawing.Color m_EnabledColor;
        private Wine_Brand m_Brand;
        private Wine_Type m_Type;
        const string c_SelectString = "--Select--";
        private System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");
        private WineSpecs m_WineSpecs = new WineSpecs();
        private int m_TxID = -1;

        public AddTransaction()
        {
            m_EnabledColor = System.Drawing.Color.FromArgb(0xE1E8F0);
            m_DisabledColor = System.Drawing.Color.FromArgb(0x999999);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["txid"] != null)
            {
                string txidstr = Request.QueryString["txid"];
                bool parsed = Int32.TryParse(txidstr, out m_TxID);
                System.Diagnostics.Debug.Assert(parsed);
            }

            if (IsPostBack)
            {
                UpdateUI();
                return;
            }
            else 
            {
                txtLastName.Focus();
                Label_BottlingDate.Text = c_SelectString;
                Label_BottlingHour.Text = c_SelectString;
                Label_BottlingStation.Text = c_SelectString;
                Label_FirstName.Text = c_SelectString;
                Label_LastName.Text = c_SelectString;
                CheckBox_EditDates.Enabled = false;
            }

            Label_TransactionID.Text = "-";
            FillWineBrands();
            FillProductCodes();
            UpdateComboBoxes();

            if (!string.IsNullOrEmpty(Request.QueryString["FromAdd"]))
            {
                // We are returning from a the rendezvous page 
                // we saved all data so now we need to restore it
                // but before get back the result of the rendezvous
                RestoreData();
                UpdateUI();

                if (m_TxID >= 0)
                    ModifyRecord();
            }
            else if (m_TxID >= 0)
            {
                EditRecord();
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            CheckBox_1.Checked = (Label_CustomerID.Text != "");
            CheckBox_1.BackColor = CheckBox_1.Checked ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            CheckBox_2.Checked = (DropDownList_Category.SelectedIndex > 0);
            CheckBox_2.BackColor = CheckBox_2.Checked ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            CheckBox_3.Checked = (Label_BottlingHour.Text != c_SelectString && Label_BottlingStation.Text != c_SelectString);
            CheckBox_3.BackColor = CheckBox_3.Checked ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            CheckBox_4.Checked = Label_TransactionID.Text != "-";
            CheckBox_4.BackColor = CheckBox_4.Checked ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            Button_Commit.Enabled = (CheckBox_1.Checked && CheckBox_2.Checked && CheckBox_3.Checked && m_TxID < 0);
            Button_Print.Enabled = (Label_TransactionID.Text != "-");
            Button_SendEmail.Enabled = Button_Print.Enabled;
        }
        private void SaveData()
        {
            Session["customer_id"] = Label_CustomerID.Text;
            Session["wine_brand"] = DropDownList_Brand.SelectedIndex;
            Session["wine_type"] = DropDownList_Type.SelectedIndex;
            Session["wine_category"] = DropDownList_Category.SelectedIndex;
            Session["rendezvous"] = Calendar_RDV.SelectedDate;
            if (Label_BottlingHour.Text != c_SelectString)
                Session["rendezvous_hour"] = Label_BottlingHour.Text;
            if (Label_BottlingStation.Text != c_SelectString)
                Session["rendezvous_station"] = Label_BottlingStation.Text;
            Session["location"] = TextBox_Location.Text;
            Session["comments"] = TextBox_Comment.Text;
            Session["product_code"] = DropDownList_ProductCode.SelectedIndex;
        }
        private void RestoreData()
        {
            if (Session["customer_id"] != null)
            {
                Customer customer = Customer.GetRecordByID((string)Session["customer_id"]);
                if (customer.id >= 0)
                {
                    Label_CustomerID.Text = customer.id.ToString();
                    Label_FirstName.Text = customer.first_name;
                    Label_LastName.Text = customer.last_name;
                }
            }
            if (Session["wine_brand"] != null)
            {
                DropDownList_Brand.SelectedIndex = (int)Session["wine_brand"];
                OnSelectBrand();
            }
            if (Session["wine_type"] != null)
            {
                DropDownList_Type.SelectedIndex = (int)Session["wine_type"];
                OnSelectType(false);
            }
            if (Session["wine_category"] != null)
            {
                DropDownList_Category.SelectedIndex = (int)Session["wine_category"];
                OnSelectCategory(false);
            }

            if (Session["rendezvous"] != null)
            {
                DateTime rdv = (DateTime)Session["rendezvous"];
                Label_BottlingDate.Text = rdv.ToString("MMM-dd-yyyy", m_Culture);
                Calendar_RDV.SelectedDate = rdv;
                Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);
            }
            if (Session["rendezvous_hour"] != null)
                Label_BottlingHour.Text = (string)Session["rendezvous_hour"];
            if (Session["rendezvous_station"] != null)
                Label_BottlingStation.Text = (string)Session["rendezvous_station"];

            if (Session["location"] != null)
                TextBox_Location.Text = (string)Session["location"];
            if (Session["comments"] != null)
                TextBox_Comment.Text = (string)Session["comments"];
            if (Session["product_code"] != null)
                DropDownList_ProductCode.SelectedIndex = (int)Session["product_code"];

            Session.Clear();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetAutoCompleteData(string pre)
        {
            return CustomersHelper.GetSimilarCustomersString(pre);
        }

        private void SelectCustomer(string name)
        {
            string customerNo;
            try
            {
                string[] words = name.Split(':');
                customerNo = words[0];
            }
            catch
            {
                ClearCustomerText();
                return;
            }

            Customer customer = Customer.GetRecordByID(customerNo);
            if (customer.id >= 0)
            {
                Label_CustomerID.Text = customer.id.ToString();
                Label_FirstName.Text = customer.first_name;
                Label_LastName.Text = customer.last_name;
            }
            else
            {
                ClearCustomerText();
            }
            txtLastName.Text = "";
        }

        void ClearCustomerText()
        {
            Label_CustomerID.Text = "";
            Label_FirstName.Text = "";
            Label_LastName.Text = "";
        }

        [WebMethod]
        public static string GetAutoCompleteDataDone(string name)
        {
            AddTransaction page = HttpContext.Current.CurrentHandler as AddTransaction;
//            page.SelectCustomer(name);
            
            return "";
        }

        protected void txtLastName_TextChanged(object sender, EventArgs e)
        {
            TextBox texBox = sender as TextBox;
            SelectCustomer(texBox.Text);
            if (m_TxID >= 0)
                ModifyRecord();
        }

        private void OnSelectBrand()
        {
            if (DropDownList_Brand.SelectedIndex > 0)
            {
                m_Brand = Wine_Brand.GetRecordByID(DropDownList_Brand.SelectedValue);
                FillWineTypes(m_Brand.id);
            }
            UpdateComboBoxes();
        }

        private void OnSelectType(bool updateDerfaultCategory)
        {
            if (DropDownList_Type.SelectedIndex > 0)
            {
                m_Type = Wine_Type.GetRecordByID(DropDownList_Type.SelectedValue);
                FillWineCategories(m_Type.category_id, updateDerfaultCategory);
            }
            UpdateComboBoxes();
        }
        private void OnSelectCategory(bool userInteration)
        {
            UpdateComboBoxes();

            if (userInteration && m_TxID >= 0)
                ModifyRecord();
        }

        protected void UpdateComboBoxes()
        {
            if (DropDownList_Brand.SelectedIndex <= 0)
            {
                DropDownList_Type.Enabled = false;
                DropDownList_Type.BackColor = m_DisabledColor;
            }
            else
            {
                DropDownList_Type.Enabled = true;
                DropDownList_Type.BackColor = m_EnabledColor;
            }

            if (DropDownList_Type.SelectedIndex <= 0)
            {
                DropDownList_Category.Enabled = false;
                DropDownList_Category.BackColor = m_DisabledColor;
            }
            else
            {
                DropDownList_Category.Enabled = true;
                DropDownList_Category.BackColor = m_EnabledColor;
            }

            if (DropDownList_Category.SelectedIndex > 0)
            {
                Wine_Category category = Wine_Category.GetRecordByID(DropDownList_Category.SelectedValue);
                Label_Price.Text = "$" + category.cost;
            }
            else
            {
                Label_Price.Text = "$" + 0;
            }
            ShowDates();
        }

        protected void FillWineBrands()
        {
            System.Data.DataSet objDs;
            m_WineSpecs.GetAllWineBrands(m_TxID >= 0, out objDs);
            if (objDs.Tables[0].Rows.Count > 0)
            {
                DropDownList_Brand.DataSource = objDs.Tables[0];
                DropDownList_Brand.DataTextField = "name";
                DropDownList_Brand.DataValueField = "id";
                DropDownList_Brand.DataBind();
                DropDownList_Brand.Items.Insert(0, c_SelectString);
            }
        }
        protected void FillWineTypes(int brandID)
        {
            System.Data.DataSet objDs;
            m_WineSpecs.GetAllWineTypes(brandID, m_TxID >= 0, out objDs);
            if (objDs.Tables[0].Rows.Count > 0)
            {
                DropDownList_Type.DataSource = objDs.Tables[0];
                DropDownList_Type.DataTextField = "name";
                DropDownList_Type.DataValueField = "id";
                DropDownList_Type.DataBind();
                DropDownList_Type.Items.Insert(0, c_SelectString);
            }
        }
        protected void FillWineCategories(int categoryID, bool selectDefaultCategory)
        {
            System.Data.DataSet objDs;
            m_WineSpecs.GetAllWineCategories(out objDs);
            if (objDs.Tables[0].Rows.Count > 0)
            {
                DropDownList_Category.DataSource = objDs.Tables[0];
                DropDownList_Category.DataTextField = "name";
                DropDownList_Category.DataValueField = "id";
                DropDownList_Category.DataBind();
                DropDownList_Category.Items.Insert(0, c_SelectString);
            }

            if (selectDefaultCategory)
            {
                // Select the category that fit the input
                for (int i = 0; i < DropDownList_Category.Items.Count; i++)
                {
                    if (DropDownList_Category.Items[i].Value == categoryID.ToString())
                    {
                        // disable selection : to validate...
                        DropDownList_Category.Enabled = false;
                        DropDownList_Category.BackColor = m_DisabledColor;

                        DropDownList_Category.SelectedIndex = i;
                        OnSelectCategory(true);
                        break;
                    }
                }
            }
        }

        protected void FillProductCodes()
        {
            System.Data.DataSet objDs;
            m_WineSpecs.GetAllProductCodes(out objDs);
            if (objDs.Tables[0].Rows.Count > 0)
            {
                DropDownList_ProductCode.DataSource = objDs.Tables[0];
                DropDownList_ProductCode.DataTextField = "name";
                DropDownList_ProductCode.DataValueField = "id";
                DropDownList_ProductCode.DataBind();
                DropDownList_ProductCode.Items.Insert(0, c_SelectString);
            }
        }

        protected void Button_Commit_Click(object sender, EventArgs e)
        {
            // CREATE RECORD
            if (Label_TransactionID.Text == "-")
                CreateRecord();
            else
                ModifyRecord();
        }

        protected void CreateRecord()
        {
            Transaction tx = new Transaction();
            FillTransactionWithScreenData(tx);

            // Create the record
            if (!Transaction.CreateRecord(tx))
            {
                Utils.MessageBox(this, "** Error **\\nFailed to create a new transaction.");
                return;
            }
            if (!TransactionsHelper.CreateStepsRecord(tx))
            {
                Utils.MessageBox(this, "** Error **\\nFailed to create a transaction steps.");
                return;
            }

            // Update UI.
            m_TxID = tx.id;
            UpdateUI();
        }

        protected DateTime GetBottlingDateFromExistingTx(Transaction tx)
        {
            Wine_Category mainCat = Wine_Category.GetRecordByID(DropDownList_Category.SelectedValue);
                
            List<Wine_Category> categories;
            int nbRecords = Wine_Category.GetAllRecordsForName(mainCat.name, out categories);
            TransactionStep yeast = TransactionStep.GetRecordForTx(m_TxID, 1);

            foreach (Wine_Category wineCat in categories)
            {
                Step step = Step.GetRecordById(wineCat.step.ToString());

                DateTime stepDate = yeast.date.AddDays(wineCat.days);
                if (step.final_step > 0 )
                {
                    return stepDate;
                }
            }
            System.Diagnostics.Debug.Assert(false);
            return DateTime.MinValue;
        }

        protected void ModifyRecord()
        {
            Transaction tx = Transaction.GetRecord(m_TxID);
            bool needToRecreateSteps = CheckTxDifferenceWithScreen(tx);

            //-----------------------------------------------------------
            // Validate if we can do that.
            // 
            if (DropDownList_Category.SelectedIndex == 0)
            {
                Utils.MessageBox(this, "** Error **\\nThe category is invalid");
                EditRecord();
                return;
            }
            else if (needToRecreateSteps && tx.IsStarted())
            {
                Utils.MessageBox(this, "** Error **\\nThe transaction is already started. The Yeast is done. You cannot change it.");
                EditRecord();
                return;
            }
            else if (needToRecreateSteps)
            {
                DateTime newBottlingDate = GetBottlingDateFromExistingTx(tx);

                // Is the new bottling date after the one in the appointment?
                Step stepDef = Step.GetLastStep();
                TransactionStep txBottlingStep = TransactionStep.GetRecordForTx(m_TxID, stepDef.id);
                if (Utils.CompareDates(newBottlingDate, tx.date_bottling) > 0)
                {
                    Utils.MessageBox(this, "** Error **\\nThe appointment date in the transaction need to be moved after " + newBottlingDate.ToString("MMM-dd-yyyy") + "  before updating this transaction");
                    EditRecord();
                    return;
                }
            }

            //-----------------------------------------------------------

            FillTransactionWithScreenData(tx);

            // Modify the record
            if (!tx.ModifyRecord())
            {
                Utils.MessageBox(this, "** Error **\\nFailed to modify a transaction.");
                EditRecord();
                return;
            }

            if (needToRecreateSteps)
            {
                // Delete all steps and recreate them.
                if (!TransactionStep.DeleteTxRecords(tx.id.ToString()))
                {
                    Utils.MessageBox(this, "** Error **\\nFailed to delete transaction steps.");
                    return;
                }

                if (!TransactionsHelper.CreateStepsRecord(tx))
                {
                    Utils.MessageBox(this, "** Error **\\nFailed to create a transaction steps.");
                    return;
                }
            }

            // Update UI.
            m_TxID = tx.id;
            UpdateUI();
        }

        protected void FillTransactionWithScreenData(Transaction tx)
        {
            int numValue;

            // Update the transaction with the values on the screen
            
            // client
            bool parsed = Int32.TryParse(Label_CustomerID.Text, out numValue);
            if (parsed && numValue > 0)
                tx.client_id = numValue;
            else
            {
                Utils.MessageBox(this, "** Error **\\nThe client is missing.");
                return;
            }

            // Brand
            parsed = Int32.TryParse(DropDownList_Brand.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_brand_id = numValue;
            else
            {
                Utils.MessageBox(this, "** Error **\\nPlease select a Brand.");
                return;
            }

            // Type
            parsed = Int32.TryParse(DropDownList_Type.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_type_id = numValue;
            else
            {
                Utils.MessageBox(this, "** Error **\\nPlease select a Type.");
                return;
            }

            // Category
            parsed = Int32.TryParse(DropDownList_Category.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_category_id = numValue;
            else
            {
                Utils.MessageBox(this, "** Error **\\nPlease select a Category.");
                return;
            }

            // Bottling
            if (Label_BottlingDate.Text != "")
            {
                tx.date_bottling = Calendar_RDV.SelectedDate;
                tx.date_bottling = tx.date_bottling.AddHours(DateHelper.GetRendezVousHour(Label_BottlingHour.Text));
                tx.date_bottling = tx.date_bottling.AddHours(DateHelper.GetRendezVousMin(Label_BottlingHour.Text));

                parsed = Int32.TryParse(Label_BottlingStation.Text, out numValue);
                System.Diagnostics.Debug.Assert(parsed);
                tx.bottling_station = numValue;
            }
            else
            {
                Utils.MessageBox(this, "** Error **\\nPlease select a Bottling date and time.");
                return;
            }

            // comments
            tx.comments = TextBox_Comment.Text;

            // location 
            tx.location = TextBox_Location.Text;

            // product code
            parsed = Int32.TryParse(DropDownList_ProductCode.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.product_code = numValue;

            tx.date_creation = DateTime.Now;
        }

        protected bool CheckTxDifferenceWithScreen(Transaction tx)
        {
            if (DropDownList_Category.SelectedValue != tx.wine_category_id.ToString())
            {
                // todo : check if the dates are different
                return true;
            }
            return false;
        }

        private void UpdateUI()
        {
            ShowDates();

            if (m_TxID >= 0)
            {
                Label_TransactionID.Text = m_TxID.ToString();
                Button_Commit.Enabled = false;
                Button_Print.Enabled = true;
                Button_SendEmail.Enabled = true;
                CheckBox_EditDates.Enabled = true;
            }
            else if (Label_TransactionID.Text != "-")
            {
                bool parsed = Int32.TryParse(Label_TransactionID.Text, out m_TxID);
                System.Diagnostics.Debug.Assert(parsed);

                Button_Commit.Enabled = false;
                Button_Print.Enabled = true;
                Button_SendEmail.Enabled = true;
                CheckBox_EditDates.Enabled = true;
            }
            else
            {
                Button_Commit.Enabled = true;
                Button_Print.Enabled = false;
                Button_SendEmail.Enabled = false;
                CheckBox_EditDates.Enabled = false;
            }
        }

        private void ShowDates()
        {
            Table_Dates.Rows.Clear();
            if (DropDownList_Category.SelectedIndex > 0)
            {
                Wine_Category mainCat = Wine_Category.GetRecordByID(DropDownList_Category.SelectedValue);
                
                List<Wine_Category> categories;
                int nbRecords = Wine_Category.GetAllRecordsForName(mainCat.name, out categories);
                
                TableRow tRowTitle = new TableRow();
                Table_Dates.Rows.Add(tRowTitle);
                TableRow tRowDates = new TableRow();
                Table_Dates.Rows.Add(tRowDates);

                DateTime selDate = Calendar_RDV.TodaysDate;
                
                List<TransactionStep> txSteps = new List<TransactionStep>();
                // Check if we have records for this transaction 
                if (m_TxID >= 0)
                {
                    Transaction tx = Transaction.GetRecord(m_TxID);
                    if ( tx != null && tx.id >= 0)
                    {
                        txSteps = TransactionStep.GetRecordsForTx(m_TxID);

                        foreach(TransactionStep step in txSteps)
                        {
                            //Title
                            TableCell tCell = new TableCell();
                            tCell.HorizontalAlign = HorizontalAlign.Center;
                            tCell.Text = step.GetStepName();
                            tCell.ForeColor = System.Drawing.Color.Black;
                            tCell.BackColor = step.done>0 ? System.Drawing.Color.Green : System.Drawing.Color.Orange;
                            tRowTitle.Cells.Add(tCell);

                            //Date
                            TableCell tCell2 = new TableCell();
                            tCell2.HorizontalAlign = HorizontalAlign.Center;
                            tCell2.Text = step.date.ToString("MMM-dd-yyyy", m_Culture);
                            tCell.BackColor = step.done>0 ? System.Drawing.Color.Green : System.Drawing.Color.Orange;
                            tRowDates.Cells.Add(tCell2);
                        }
                    }
                }
                else // no records yet so let just guess the dates.
                {
                    foreach (Wine_Category wineCat in categories)
                    {
                        Step step = Step.GetRecordById(wineCat.step.ToString());

                        DateTime stepDate = selDate.AddDays(wineCat.days);
                        if (step.final_step > 0 && Label_BottlingDate.Text == c_SelectString)
                        {
                            Calendar_RDV.SelectedDate = stepDate;
                            Calendar_RDV.VisibleDate = new DateTime(stepDate.Year, stepDate.Month, 1);
                        }

                        //Title
                        TableCell tCell = new TableCell();
                        tCell.HorizontalAlign = HorizontalAlign.Center;
                        tCell.Text = step.name;
                        tCell.ForeColor = System.Drawing.Color.Black;
                        tRowTitle.Cells.Add(tCell);

                        //Date
                        TableCell tCell2 = new TableCell();
                        //tCell2.ForeColor = System.Drawing.Color;
                        tCell2.HorizontalAlign = HorizontalAlign.Center;
                        tCell2.Text = stepDate.ToString("MMM-dd-yyyy", m_Culture);
                        tRowDates.Cells.Add(tCell2);
                    }
                }
            }
        }

        protected void Button_SelectDate_Click(object sender, EventArgs e)
        {
            SaveData();
            string arguments = Label_FirstName.Text + " " + Label_LastName.Text;
            Response.Redirect("~/Transactions/Rendezvous.aspx?FromAddTx=true&customer=" + arguments);
        }

        void EditRecord()
        {
            Transaction tx = Transaction.GetRecord(m_TxID);
            Customer custo = Customer.GetRecordByID(tx.client_id.ToString());

            Label_TransactionID.Text = tx.id.ToString();
            Label_CustomerID.Text = tx.client_id.ToString();

            Label_FirstName.Text = custo.first_name;
            Label_LastName.Text = custo.last_name;

            DropDownList_Brand.SelectedValue = tx.wine_brand_id.ToString();
            FillWineTypes(tx.wine_brand_id);
            DropDownList_Type.SelectedValue = tx.wine_type_id.ToString();
            FillWineCategories(tx.wine_category_id, false);
            DropDownList_Category.SelectedValue = tx.wine_category_id.ToString();
            UpdateComboBoxes();

            Label_BottlingHour.Text = DateHelper.GetHFormatedDate(tx.date_bottling);
            Label_BottlingStation.Text = tx.bottling_station.ToString();
            DateTime rdv = new DateTime(tx.date_bottling.Year, tx.date_bottling.Month, tx.date_bottling.Day);
            Label_BottlingDate.Text = rdv.ToString("MMM-dd-yyyy", m_Culture);
            Calendar_RDV.SelectedDate = rdv;
            Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);

            // we may have a product code that is not valid
            try
            {
                TextBox_Comment.Text = tx.comments;
                TextBox_Location.Text = tx.location;
                DropDownList_ProductCode.SelectedValue = tx.product_code.ToString();
            }
            catch { }

            UpdateUI();
            SaveData();
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            if (Label_TransactionID.Text != "-")
            {
                Response.Redirect("~/Transactions/AddTransaction_Print.aspx?Tx=" + Label_TransactionID.Text);
            }
        }

        protected void Button_SendEmail_Click(object sender, EventArgs e)
        {
            if (Label_TransactionID.Text != "-")
            {
                Utils.MessageBox(this, "Not yet implemented");  
            }
        }

///////////////////////////////////
// Change dates
///////////////////////////////////
        protected void DropDownList_Steps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_TxID == -1)
                return;

            int stepID=0;
            Int32.TryParse(DropDownList_Steps.SelectedValue, out stepID);
            TransactionStep txStep = TransactionStep.GetRecordForTx(m_TxID, stepID);

            TextBox_Date.Text = txStep.date.ToString("MMM-dd-yyyy");
        }

        protected void CheckBox_EditDates_CheckedChanged(object sender, EventArgs e)
        {
            DropDownList_Steps.Visible = CheckBox_EditDates.Checked;
            TextBox_Date.Visible = CheckBox_EditDates.Checked;
            Button_ChangeDate.Visible = CheckBox_EditDates.Checked;
            if (CheckBox_EditDates.Checked && TextBox_Date.Text == "")
            {
                TransactionStep txStep = TransactionStep.GetRecordForTx(m_TxID, 1); //set yeast by default
                TextBox_Date.Text = txStep.date.ToString("MMM-dd-yyyy");
            }
        }

        protected void Button_ChangeDate_Click(object sender, EventArgs e)
        {
            if (m_TxID == -1)
                return;

            Transaction tx = Transaction.GetRecord(m_TxID);

            DateTime date;
            if (!DateTime.TryParse(TextBox_Date.Text, out date))
            {
                Utils.MessageBox(this, "** Error **\\nCannot change date.\\nInvalid input date");
                return;
            }

            int stepID=0;
            Int32.TryParse(DropDownList_Steps.SelectedValue, out stepID);
            TransactionStep txStep = TransactionStep.GetRecordForTx(m_TxID, stepID); 
            
            // Validation1: is step done?
            if (txStep.done > 0)
            {
                Utils.MessageBox(this, "** Error **\\nCannot change date.\\nTransactionStep is completed.");
                return;
            }

            if (Utils.CompareDates(date, txStep.date) > 0)
            {
                // Is the new bottling date after the one in the appointment?
                DateTime newBottlingDate = TransactionsHelper.GetFinalStepDate(tx, date, txStep);

                if (Utils.CompareDates(newBottlingDate, tx.date_bottling) > 0)
                {
                    Utils.MessageBox(this, "** Error **\\nCannot change date.\\nThe appointment date in the transaction need to be moved after " + newBottlingDate.ToString("MMM-dd-yyyy"));
                    return;
                }
                else if (Utils.CompareDates(newBottlingDate, tx.date_bottling) == 0)
                {
                    Utils.MessageBox(this, "** Error **\\nCannot change date.\\nThe appointment date is the same. " + newBottlingDate.ToString("MMM-dd-yyyy"));
                    return;
                }
            }
            else if (Utils.CompareDates(date, txStep.date) == 0)
            {
                Utils.MessageBox(this, "** Error **\\nCannot change date.\\nThe date is the same");
                return;
            }

            // Now do change all other dates
            // this commit the dates
            string datesAdjusted;
            if (TransactionsHelper.UpdateStepsRecordDate(tx, stepID, date, out datesAdjusted))
            {
                ShowDates();
                if (datesAdjusted != null)
                    Utils.MessageBox(this, "Success : The date has been changed.\\nThe dates for ulterior steps have also been updated: " + datesAdjusted);
                else
                    Utils.MessageBox(this, "Success : The date has been changed.");
            }
            else
            {
                Utils.MessageBox(this, "** Fatal Error** : \\nCannot change date.\\nAn error occurred while changing the dates. please delete the transaction and recreate it. ");
            }
        }

        ///////////////////////////////////
        // Interface callbacks
        ///////////////////////////////////
        protected void DropDownList_Brand_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectBrand();
        }

        protected void DropDownList_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectType(true);
        }

        protected void DropDownList_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectCategory(true);

            // When we select a category the rendez vous is not good anymore
            //if (Label_BottlingDate.Text != c_SelectString)
            //{
            //    Label_BottlingDate.Text = c_SelectString;
            //    Label_BottlingHour.Text = c_SelectString;
            //    Label_BottlingStation.Text = c_SelectString;
            //    Utils.MessageBox(this, "Wine Category changed. ***You need to set a new appointment.***");  //TODO not working

            //    // Need to advise the customer
            //}
            //else
            //{
            //    // Pick the automatic rendezvous date if it is not done yet
            //    RendezVous rendezvous = new RendezVous();
            //    DateTime rdvDate = rendezvous.PickBestAvailableBottlingDate(Calendar_RDV.SelectedDate);
            //    Label_BottlingDate.Text = rdvDate.ToString("MMM-dd-yyyy", m_Culture);
            //    //todo add the hour and station
            //}
        }

        protected void DropDownList_ProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_TxID >= 0)
                ModifyRecord();
        }

        protected void TextBox_Comment_TextChanged(object sender, EventArgs e)
        {
            if (m_TxID >= 0)
                ModifyRecord();
        }

        protected void TextBox_Location_TextChanged(object sender, EventArgs e)
        {
            if (m_TxID >= 0)
                ModifyRecord();
        }
    }
}

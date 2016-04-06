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
        private const string c_PrefixDateID = "Date";
        private const string c_PrefixDoneID = "Done";

        private System.Drawing.Color m_DisabledColor;
        private System.Drawing.Color m_EnabledColor;
        private Wine_Brand m_Brand;
        private Wine_Type m_Type;
        const string c_SelectString = "--Select--";
        private System.Globalization.CultureInfo m_Culture = new System.Globalization.CultureInfo("en-us");
        private WineSpecs m_WineSpecs = new WineSpecs();
        private int m_TxID = -1;
        private List<string> m_AllIds = new List<string>();

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

            if (Request.QueryString["alltxids"] != null)
            {
                m_AllIds = Request.QueryString["alltxids"].Split(',').ToList();
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
                Label_Tel.Text = c_SelectString;

                Label_Iterator.Visible = false;
                Button_Next.Visible = false;
                Button_Previous.Visible = false;
            }

            Label_TransactionID.Text = "-";
            Label_CreationDate.Text = "";
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
            Button_ResetDate.Visible = Button_Print.Enabled;
            CheckBox_TxCompleted.Visible = Button_Print.Enabled;
            UpdateTxCompletCheckbox();

            Button_Duplicate.Visible = Button_Print.Enabled;
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

            if (m_TxID >=0 )
            {
                TransactionStep step = TransactionStep.GetRecordForTx(m_TxID, Step.GetLastStep().id);
                if (step != null)
                    Session["wineready_date"] = step.date;
            }
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
                    Label_Tel.Text = customer.telephone;
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
                Label_Tel.Text = customer.telephone;
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
            Label_Tel.Text = "";
        }

        [WebMethod]
        public static string GetAutoCompleteDataDone(string name)
        {
            AddTransaction page = HttpContext.Current.CurrentHandler as AddTransaction;
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
                    Utils.MessageBox(this, "** Warning **\\nThe appointment date is now before the wine ready date.\\nThe appointment date in the transaction would need to be moved after: " + newBottlingDate.ToString("MMM-dd-yyyy") + "  before updating this transaction");
                    //EditRecord();
                    //return;
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

                // Success
                Utils.MessageBox(this, "** Success **\\nThe transaction steps of transaction: " + tx.id.ToString() + " have been recreated to match the new category recipes.");
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
                tx.date_bottling = tx.date_bottling.AddMinutes(DateHelper.GetRendezVousMin(Label_BottlingHour.Text));

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

            if (Label_TransactionID.Text != "-" && m_TxID < 0)
            {
                bool parsed = Int32.TryParse(Label_TransactionID.Text, out m_TxID);
                System.Diagnostics.Debug.Assert(parsed);
            }

            if (m_TxID >= 0)
            {
                Transaction tx = Transaction.GetRecord(m_TxID);
                Label_TransactionID.Text = m_TxID.ToString();
                Label_CreationDate.Text = "Created: " + tx.date_creation.ToString("MMM-dd-yyyy", m_Culture);
                Button_Commit.Enabled = false;
                Button_Print.Enabled = true;
                Button_SendEmail.Enabled = true;
                Button_ResetDate.Visible = true;
                CheckBox_TxCompleted.Visible = true;
                UpdateTxCompletCheckbox();

                if (m_AllIds.Count > 0)
                {
                    Label_Iterator.Visible = true;
                    Button_Next.Visible = true;
                    Button_Previous.Visible = true;
                    Button_Previous.ImageUrl = "~/images/previous.png";
                    Button_Next.ImageUrl = "~/images/next.png";

                    // Find wich element we are now.
                    for (int i=0; i<m_AllIds.Count; i++ )
                    {
                        if (m_AllIds[i]== m_TxID.ToString())
                        {
                            Label_Iterator.Text = (i+1).ToString() + "/" + m_AllIds.Count.ToString();
                            if (i == 0)
                            {
                                Button_Previous.Enabled = false;
                                Button_Previous.ImageUrl = "~/images/previous_disabled.png";
                            }
                            if (i == (m_AllIds.Count - 1))
                            {
                                Button_Next.Enabled = false;
                                Button_Next.ImageUrl = "~/images/next_disabled.png";
                            }
                            break;
                        }
                    }
                }
                else
                {
                    Label_Iterator.Visible = false;
                    Button_Next.Visible = false;
                    Button_Previous.Visible = false;
                }
            }
            else
            {
                Button_Commit.Enabled = true;
                Button_Print.Enabled = false;
                Button_SendEmail.Enabled = false;
                Button_ResetDate.Visible = false;
                CheckBox_TxCompleted.Visible = false;
                Label_Iterator.Visible = false;
                Button_Next.Visible = false;
                Button_Previous.Visible = false;
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
                Table_Dates.CellSpacing = 0;
                Table_Dates.CellPadding = 0;
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

                        int nameIndex = 0;
                        foreach(TransactionStep step in txSteps)
                        {
                            //Title
                            TableCell tCell = new TableCell();
                            tCell.HorizontalAlign = HorizontalAlign.Center;
                            tCell.Text = step.GetStepName();
                            tCell.ForeColor = System.Drawing.Color.Black;
                            tCell.BackColor = step.done>0 ? System.Drawing.Color.Green : System.Drawing.Color.Orange;
                            tCell.RowSpan = 0;
                            tRowTitle.Cells.Add(tCell);
                            
                            //Date
                            TableCell tCell2 = new TableCell();
                            tCell2.HorizontalAlign = HorizontalAlign.Center;

                            // Add a textBox
                            TextBox textBoxCtrl = new TextBox();
                            tCell2.Controls.Add(textBoxCtrl);
                            textBoxCtrl.Text = step.date.ToString("MMM-dd-yyyy", m_Culture);
                            textBoxCtrl.Font.Size = 9;
                            textBoxCtrl.Width = 70;
                            textBoxCtrl.CssClass = "TheDateTimePicker";
                            textBoxCtrl.TextChanged += new EventHandler(this.OnDateChangeByUser);
                            textBoxCtrl.ID = c_PrefixDateID + step.step_id.ToString();
                            textBoxCtrl.AutoPostBack = true;
                            textBoxCtrl.BorderWidth = 1;
                            if (step.done > 0)
                                textBoxCtrl.BackColor = System.Drawing.Color.Gray;
                            textBoxCtrl.ToolTip = "Date of the step. Click in it to get the calendar to select a date. The record is modified as soon as you change the date.\nBe careful, changing the yeast (only) will change dates of all steps if not completed to fit the recipes.";

                            // Add a CheckBox 
                            CheckBox cb = new CheckBox();
                            tCell2.Controls.Add(cb);
                            cb.Checked = step.done > 0;
                            cb.AutoPostBack = true;
                            cb.ID = c_PrefixDoneID + step.step_id.ToString();
                            cb.CheckedChanged += new EventHandler(this.OnTxStepChangedByUser);
                            cb.ToolTip = "When checked the step is completed. You can change the status by clicking the checkbox. The record is modified as soon you click.";

                            tCell2.RowSpan = 0;
                            tCell2.BackColor = step.done>0 ? System.Drawing.Color.Green : System.Drawing.Color.Orange;
                            tRowDates.Cells.Add(tCell2);

                            nameIndex++;
                        }

                        // Check if the recipes is broken
                        Label_BrokenRecipes.Visible = TransactionsHelper.CheckBrokenRecipes(tx);
                    }
                }
                else // no records yet so let just guess the dates.
                {
                    foreach (Wine_Category wineCat in categories)
                    {
                        Step step = Step.GetRecordById(wineCat.step.ToString());
                        if (step == null)
                            continue;

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
            if (m_TxID >= 0)
            {
                arguments += "&txid=" + m_TxID.ToString();
                if (m_AllIds.Count>0)
                    arguments += "&alltxids=" + string.Join(",", m_AllIds);
            }
            
            Response.Redirect("~/Transactions/Rendezvous.aspx?FromAddTx=true&customer=" + arguments);
        }

        void EditRecord()
        {
            Transaction tx = Transaction.GetRecord(m_TxID);
            Customer custo = Customer.GetRecordByID(tx.client_id.ToString());

            Label_TransactionID.Text = tx.id.ToString();
            Label_CreationDate.Text = "Created: " + tx.date_creation.ToString("MMM-dd-yyyy", m_Culture);
            Label_CustomerID.Text = tx.client_id.ToString();

            Label_FirstName.Text = custo.first_name;
            Label_LastName.Text = custo.last_name;
            Label_Tel.Text = custo.telephone;

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

        protected void UpdateTxCompletCheckbox()
        {
            if (CheckBox_TxCompleted.Visible)
            {
                Transaction tx = Transaction.GetRecord(m_TxID);
                if (tx != null)
                {
                    int nbStepDone = 0;
                    int nbStepTotal = 0;
                    CheckBox_TxCompleted.Enabled = tx.AreAllStepDone(out nbStepDone, out nbStepTotal);
                    if (!IsPostBack)
                        CheckBox_TxCompleted.Checked = tx.done;
                }
            }
        }

///////////////////////////////////
// Change dates
///////////////////////////////////

        private void OnDateChangeByUser(Object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(m_TxID >= 0);
            TextBox textBox = sender as TextBox;

            DateTime date;
            if (!DateTime.TryParse(textBox.Text, out date))
            {
                Utils.MessageBox(this, "** Error **\\nCannot change date.\\nInvalid input date");
                return;
            }
            int stepID = 0;
            string id = textBox.ID.Replace(c_PrefixDateID, "");
            bool parsed = Int32.TryParse(id, out stepID);
            System.Diagnostics.Debug.Assert(parsed);

            // Update the date directly
            ChangeTxStepDates(date, stepID);
        }

        private void OnTxStepChangedByUser(Object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(m_TxID >= 0);
            CheckBox cb = sender as CheckBox;

            int stepID = 0;
            string id = cb.ID.Replace(c_PrefixDoneID, "");
            bool parsed = Int32.TryParse(id, out stepID);
            System.Diagnostics.Debug.Assert(parsed);

            Transaction tx = Transaction.GetRecord(m_TxID);
            if (tx != null)
            {
                TransactionStep step = TransactionStep.GetRecordForTx(m_TxID, stepID);
                if (step != null)
                {
                    step.done = cb.Checked ? 1 : 0;
                    step.ModifyRecord();
                    ShowDates();
                }
            }
        }

        protected void Button_ResetDate_Click(object sender, EventArgs e)
        {
            // Reset the Yeast date starting today
            ChangeTxStepDates(DateTime.Now, Step.c_YeastID);
        }

        protected void ChangeTxStepDates(DateTime date, int stepID )
        {
            if (m_TxID == -1)
                return;
            Transaction tx = Transaction.GetRecord(m_TxID);
            TransactionStep txStep = TransactionStep.GetRecordForTx(m_TxID, stepID);
            DateTime originalDate = txStep.date;
            
            // Validation1: is step done?
            if (txStep.done > 0)
            {
                Utils.MessageBox(this, "** Warning **\\nYou are changing a TransactionStep that is completed.");
                //return;  //now allow it
            }

            // Date pushing Validation. Only yeast is pushing dates.
            if (stepID == Step.c_YeastID && Utils.CompareDates(date, txStep.date) > 0)
            {
                // Is the new bottling date after the one in the appointment?
                DateTime newBottlingDate = TransactionsHelper.GetFinalStepDate(tx, date, txStep);

                if (Utils.CompareDates(newBottlingDate, tx.date_bottling) > 0)
                {
                    Utils.MessageBox(this, "** Warning **\\nThe appointment date now before the wine is ready.\\n\\nWine ready new date: " + newBottlingDate.ToString("MMM-dd-yyyy") + "\\nAppointement date:" + tx.date_bottling.ToString("MMM-dd-yyyy"));
                    //return;  //now allow it
                }
            }
            else if (Utils.CompareDates(date, txStep.date) == 0)
            {
                Utils.MessageBox(this, "** Error **\\nCannot change date.\\nThe date is the same");
                return;
            }

            if (stepID == Step.c_YeastID)
            {
                // Now do change all other dates
                // this commit the dates
                string datesAdjusted;
                if (TransactionsHelper.UpdateAndAdjustStepsRecordDate(tx, stepID, date, out datesAdjusted))
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
            else
            {
                if (TransactionsHelper.UpdateStepRecordDate(tx, stepID, date))
                {
                    ShowDates();
                    string msg = "Success : The date of step " + Step.GetStepName(stepID) +
                                 " has been changed.\\n\\nBefore: " + originalDate.ToString("MMM-dd-yyyy") +
                                 "\\nAfter:" + date.ToString("MMM-dd-yyyy");
                    Utils.MessageBox(this, msg);

                    Step lastStep = Step.GetLastStep();
                    if (stepID != lastStep.id)
                        Utils.MessageBox(this, "** Warning **\\nThe date of step " + Step.GetStepName(stepID) + " changed. This transaction doesn't follow the recipes anymore.");
                }
                else
                {
                    Utils.MessageBox(this, "** Error ** : \\nCannot change date.\\nAn error occurred while changing the dates.");
                }
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

        public void OnConfirm(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked YES!')", true);
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked NO!')", true);
            }
        }

        protected void Button_Duplicate_Click(object sender, EventArgs e)
        {
            // Create a new transaction but with the same fields.
            m_TxID = -1;
            Label_TransactionID.Text = "-";
            Label_CreationDate.Text = "";

            UpdateUI();
        }

        protected void CheckBox_TxCompleted_CheckedChanged(object sender, EventArgs e)
        {
            if (m_TxID < 0)
                return;

            List<string> ids = new List<string>();
            ids.Add(m_TxID.ToString());
            bool res = TransactionsHelper.SetTransactionToDone(ids, CheckBox_TxCompleted.Checked); 
            if (res)
            {
                if (CheckBox_TxCompleted.Checked)
                    Utils.MessageBox(this, "** Success **\\nThe transaction is now completed.");
                else
                    Utils.MessageBox(this, "** Success **\\nThe transaction is NOT completed anymore.");
            }
            else
                Utils.MessageBox(this, "** Error **\\nFailed to create a new transaction.");
        }

        protected void Button_Next_Click(object sender, ImageClickEventArgs e)
        {
            for (int i = 0; i < m_AllIds.Count; i++)
            {
                if (m_AllIds[i] == m_TxID.ToString())
                {
                    System.Diagnostics.Debug.Assert(i+1 < m_AllIds.Count);
                    if (i >= m_AllIds.Count)
                        return;

                    string newtxid = m_AllIds[i + 1];

                    string url = Utils.ResolveServerUrl("/Transactions/AddTransaction.aspx", false);
                    url += "?txid=" + newtxid;
                    url += "&alltxids=" + string.Join(",", m_AllIds);
                    Response.Redirect(url);
                    break;
                }
            }
        }

        protected void Button_Previous_Click(object sender, ImageClickEventArgs e)
        {
            for (int i = 0; i < m_AllIds.Count; i++)
            {
                if (m_AllIds[i] == m_TxID.ToString())
                {
                    System.Diagnostics.Debug.Assert(i > 0 );
                    string newtxid = m_AllIds[i - 1];

                    string url = Utils.ResolveServerUrl("/Transactions/AddTransaction.aspx", false);
                    url += "?txid=" + newtxid;
                    url += "&alltxids=" + string.Join(",", m_AllIds);
                    Response.Redirect(url);
                    break;
                }
            }
        }
    }
}

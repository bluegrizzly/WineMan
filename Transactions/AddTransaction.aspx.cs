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

                if (Request.QueryString["txid"] != null)
                {
                    string txidstr = Request.QueryString["txid"];
                    bool parsed = Int32.TryParse(txidstr, out m_TxID);
                    System.Diagnostics.Debug.Assert(parsed);
                }
            }

            Label_TransactionID.Text = "-";
            FillWineBrands();
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

            Button_Commit.Enabled = (CheckBox_1.Checked && CheckBox_2.Checked && CheckBox_3.Checked);
            Button_Print.Enabled = (Label_TransactionID.Text != "-");
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
                OnSelectType();
            }
            if (Session["wine_category"] != null)
            {
                DropDownList_Category.SelectedIndex = (int)Session["wine_category"];
                OnSelectCategory();
            }

            if (Session["rendezvous"] != null)
            {
                DateTime rdv = (DateTime)Session["rendezvous"];
                Label_BottlingDate.Text = rdv.ToString("d", m_Culture);
                Calendar_RDV.SelectedDate = rdv;
                Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);
            }
            if (Session["rendezvous_hour"] != null)
            {
                Label_BottlingHour.Text = (string)Session["rendezvous_hour"];
            }
            if (Session["rendezvous_station"] != null)
            {
                Label_BottlingStation.Text = (string)Session["rendezvous_station"];
            }

            Session.Clear();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetAutoCompleteData(string pre)
        {
            return CustomersHelper.GetSimilarCustomers(pre);
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

        private void OnSelectType()
        {
            if (DropDownList_Type.SelectedIndex > 0)
            {
                m_Type = Wine_Type.GetRecordByID(DropDownList_Type.SelectedValue);
                FillWineCategories(m_Type.category_id);
            }
            UpdateComboBoxes();
        }
        private void OnSelectCategory()
        {
            UpdateComboBoxes();
        }
        //
        protected void DropDownList_Brand_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectBrand();
        }

        protected void DropDownList_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectType();
        }

        protected void DropDownList_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectCategory();

            // when we select a category the rendez vous is not good anymore
            if (Label_BottlingDate.Text != c_SelectString)
            {
                Label_BottlingDate.Text = c_SelectString;
                Label_BottlingHour.Text = c_SelectString;
                Label_BottlingStation.Text = c_SelectString;
                Utils.MessageBox(this, "Wine Category changed.\n***You need to set a new appointment.***");  //TODO not working
            }
            else  
            {
                // Pick the automatic rendezvous date if it is not done yet
                RendezVous rendezvous = new RendezVous();
                DateTime rdvDate = rendezvous.PickBestAvailableBottlingDate(Calendar_RDV.SelectedDate);
                Label_BottlingDate.Text = rdvDate.ToString("d", m_Culture);
                //todo add the hour and station
            }
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
            m_WineSpecs.GetAllWineBrands(out objDs);
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
            m_WineSpecs.GetAllWineTypes(brandID, out objDs);
            if (objDs.Tables[0].Rows.Count > 0)
            {
                DropDownList_Type.DataSource = objDs.Tables[0];
                DropDownList_Type.DataTextField = "name";
                DropDownList_Type.DataValueField = "id";
                DropDownList_Type.DataBind();
                DropDownList_Type.Items.Insert(0, c_SelectString);
            }
        }
        protected void FillWineCategories(int categoryID)
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

            // Select the category that fit the input
            for (int i=0; i<DropDownList_Category.Items.Count; i++)
            {
                if (DropDownList_Category.Items[i].Value == categoryID.ToString())
                {
                    // disable selection : to validate...
                    DropDownList_Category.Enabled = false;
                    DropDownList_Category.BackColor = m_DisabledColor;

                    DropDownList_Category.SelectedIndex = i;
                    break;
                }
            }
        }

        protected void Button_Commit_Click(object sender, EventArgs e)
        {
            // CREATE RECORD
            if (Label_TransactionID.Text == "-")
            {
                Transaction tx = new Transaction();
                FillTransactionWithScreenData(tx);

                // Create the record
                if (!Transaction.CreateRecord(tx))
                {
                    Utils.MessageBox(this, "Error: Failed to create a new transaction.");
                    return;
                }
                if (!TransactionsHelper.CreateStepsRecord(tx))
                {
                    Utils.MessageBox(this, "Error: Failed to create a transaction steps.");
                    return;
                }

                // Update UI.
                m_TxID = tx.id;
                UpdateUI();
            }
            // MODIFY RECORD
            else 
            {
                //Utils.MessageBox(this, "Not yet implemented.");
                //return;
                Transaction tx = Transaction.GetRecord(m_TxID);
                bool needToRecreateSteps = CheckTxDifferenceWithScreen(tx);
                FillTransactionWithScreenData(tx);

                // Create the record
                if (!tx.ModifyRecord())
                {
                    Utils.MessageBox(this, "Error: Failed to modify a transaction.");
                    return;
                }

                if (needToRecreateSteps)
                {
                    // Delete all steps and recreate them.
                    if (!TransactionStep.DeleteTxRecords(tx.id))
                    {
                        Utils.MessageBox(this, "Error: Failed to delete transaction steps.");
                        return;
                    }

                    if (!TransactionsHelper.CreateStepsRecord(tx))
                    {
                        Utils.MessageBox(this, "Error: Failed to create a transaction steps.");
                        return;
                    }
                }

                // Update UI.
                m_TxID = tx.id;
                UpdateUI();
            }
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
                Utils.MessageBox(this, "Error: The client is missing.");
                return;
            }

            // Brand
            parsed = Int32.TryParse(DropDownList_Brand.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_brand_id = numValue;
            else
            {
                Utils.MessageBox(this, "Error: Please select a Brand.");
                return;
            }

            // Type
            parsed = Int32.TryParse(DropDownList_Type.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_type_id = numValue;
            else
            {
                Utils.MessageBox(this, "Error: Please select a Type.");
                return;
            }

            // Category
            parsed = Int32.TryParse(DropDownList_Category.SelectedValue, out numValue);
            if (parsed && numValue > 0)
                tx.wine_category_id = numValue;
            else
            {
                Utils.MessageBox(this, "Error: Please select a Category.");
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
                Utils.MessageBox(this, "Error: Please select a Bottling date and time.");
                return;
            }

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
                Button_Commit.Text = "Modify";
                Button_Print.Enabled = true;
            }
            else if (Label_TransactionID.Text != "-")
            {
                bool parsed = Int32.TryParse(Label_TransactionID.Text, out m_TxID);
                System.Diagnostics.Debug.Assert(parsed);
                
                Button_Commit.Text = "Modify";
                Button_Print.Enabled = true;
            }
            else
            {
                Button_Commit.Text = "Create";
                Button_Print.Enabled = false;
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

                foreach (Wine_Category wineCat in categories)
                {
                    Step step = Step.GetRecord(wineCat.step.ToString());

                    DateTime stepDate = selDate.AddDays(wineCat.days);
                    if (step.final_step>0 && Label_BottlingDate.Text == c_SelectString)
                    {
                        Calendar_RDV.SelectedDate= stepDate;
                        Calendar_RDV.VisibleDate = new DateTime(stepDate.Year,stepDate.Month,1);
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
            FillWineCategories(tx.wine_category_id);
            DropDownList_Category.SelectedValue = tx.wine_category_id.ToString();
            UpdateComboBoxes();

            Label_BottlingHour.Text = DateHelper.GetHFormatedDate(tx.date_bottling);
            Label_BottlingStation.Text = tx.bottling_station.ToString();
            DateTime rdv = new DateTime(tx.date_bottling.Year, tx.date_bottling.Month, tx.date_bottling.Day);
            Label_BottlingDate.Text = rdv.ToString("d", m_Culture);
            Calendar_RDV.SelectedDate = rdv;
            Calendar_RDV.VisibleDate = new DateTime(Calendar_RDV.SelectedDate.Year, Calendar_RDV.SelectedDate.Month, 1);

            UpdateUI();
            SaveData();
        }

        protected void Button_Print_Click(object sender, EventArgs e)
        {
            if (Label_TransactionID.Text != "-")
                Response.Redirect("~/Transactions/AddTransaction_Print.aspx?Tx=" + Label_TransactionID.Text);
        }
    }
}


using ExcelDataReader;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using Z.Dapper.Plus;
namespace QuanLyQuanKaraoke//QuanLyCafe
{
    public partial class frAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource tableList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        public Account loginAccount;



        public frAdmin()
        {
            InitializeComponent();
            Loading();

        }
        List<Food_DTO> SearchFoodByName(string name)
        {
            List<Food_DTO> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }
        void Loading()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvTable.DataSource = tableList;
            dtgvCategory.DataSource = categoryList;
            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadListFoodCategory();
            LoadAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            LoadTable();
            AddFoodBinding();
            AddFoodCategoryBinding();
            AddAccountBinding();
            AddTableBinding();

        }
        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            numericUpDown1.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }
        void LoadListFoodCategory()
        {
            categoryList.DataSource = FoodCategoryDAO.Instance.GetListFoodCategory1();
        }
        void LoadTable()
        {
            tableList.DataSource = TableDAO.Instance.LoadTableList1();
        }
        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }
        void AddFoodCategoryBinding()
        {
            txtNameCategory.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txtIdCategory.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
        }
        void AddTableBinding()
        {
            txtNameTable.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txtIdTable.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtStatusTable.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
        }
        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }
        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Vui lòng đừng xóa chính bạn ");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }

            LoadAccount();
        }
        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }
        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    Category cateogory = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = cateogory;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == cateogory.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { MessageBox.Show("Lỗi tìm kiếm !! bạn hãy thử lần sau với tên món ăn cần tìm..", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text) ;
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(id , name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm thức ăn");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);
            if (MessageBox.Show("Bạn có chắc chắn xóa món ăn " + txbFoodName.Text + " này ?", "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (FoodDAO.Instance.DeleteFood(id))
                {
                    MessageBox.Show("Xóa món thành công");
                    LoadListFood();
                    if (deleteFood != null)
                        deleteFood(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa thức ăn");
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txbUserName.Text;
                string displayName = txbDisplayName.Text;
                int type = (int)numericUpDown1.Value;

                AddAccount(userName, displayName, type);
            }
            catch
            {
                MessageBox.Show("Bạn cần trỏ vào khoản trống thì mới thực thi Thêm !!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            if (MessageBox.Show("Bạn có chắc chắn xóa tài khoàn  " + txbUserName.Text + " này ?", "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DeleteAccount(userName);
            }
        }

        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDown1.Value;

            EditAccount(userName, displayName, type);
        }

        private void btnRessetPassWord_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            ResetPass(userName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                frReport f = new frReport();
                f.ShowDialog();
            }
            catch
            { MessageBox.Show("Mở Không Thành Công!! \n 1: Cập nhật lại CrystalReports \n 2: Nên tự tao mẫu báo cáo dạng thường ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListFoodCategory();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txtNameCategory.Text;

            if (FoodCategoryDAO.Instance.InsertFoodCategory(name))
            {
                MessageBox.Show("Thêm danh mục thành công");
                LoadListFoodCategory();
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm danh mục");
            }
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIdCategory.Text);
            if (MessageBox.Show("Bạn có chắc chắn xóa danh mục món ăn " + txtNameCategory.Text, "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (FoodCategoryDAO.Instance.DeleteFoodCategory(id))
                {
                    MessageBox.Show("Xóa danh mục thành công");
                    LoadListFoodCategory();
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa danh mục");
                }
            }
        }

        private void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            string name = txtNameCategory.Text;
            int id = Convert.ToInt32(txtIdCategory.Text);

            if (FoodCategoryDAO.Instance.UpdateFoodCategory(name, id))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFoodCategory();
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }
        }

        private void btnShowTable_Click(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txtNameTable.Text;
            string status = txtStatusTable.Text;

            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Thêm bàn thành công");
                LoadTable();

            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm bàn");
            }
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIdTable.Text);
            if (MessageBox.Show("Bạn có chắc chắn xóa " + txtNameTable.Text + " này ?", "Thông báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (TableDAO.Instance.DeleteTable(id))
                {
                    MessageBox.Show("Xóa bàn thành công");
                    LoadTable();
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa bàn");
                }
            }
        }

        private void btnUpdateTable_Click(object sender, EventArgs e)
        {
            string name = txtNameTable.Text;
            string status = txtStatusTable.Text;
            int id = Convert.ToInt32(txtIdTable.Text);

            if (TableDAO.Instance.UpdateTable(name, id))
            {
                MessageBox.Show("Sửa bàn thành công");
                LoadTable();
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa bàn ăn");
            }
        }

        private void bntAdd1_Click(object sender, EventArgs e)
        {
            try
            {
                string userName = txbUserName.Text;
                string displayName = txbDisplayName.Text;
                int type = (int)numericUpDown1.Value;

                AddAccount(userName, displayName, type);
            }
            catch
            {
                MessageBox.Show("Bạn cần trỏ vào khoản trống thì mới thực thi Thêm !!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            numericUpDown1.Value = 0;
            if (txbUserName.Text != string.Empty && txbDisplayName.Text != string.Empty)
            {
               
                bntAdd1.Enabled = false;
                btnRessetPassWord.Enabled = true;
                btnUpdateAccount.Enabled = true;
                btnDeleteAccount.Enabled = true;
                btnAddAccount.Enabled = false;
            }
            else
            {
                bntAdd1.Enabled = true;
                btnAddAccount.Enabled = true;
                btnRessetPassWord.Enabled = false;
                btnUpdateAccount.Enabled = false;
                btnDeleteAccount.Enabled = false;
            }
        }

        private void dtgvAccount_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void trangthaiClick()
        {
            this.dtgvAccount.CellClick -= this.dtgvAccount_CellClick;
            dtgvAccount.Refresh();
            this.dtgvTable.CellClick -= this.dtgvTable_CellClick;
            dtgvTable.Refresh();
            this.dtgvCategory.CellClick -= this.dtgvCategory_CellClick;
            dtgvCategory.Refresh();
            LoadAccount();
            LoadTable();
            LoadListFoodCategory();
            this.dtgvAccount.CellClick += this.dtgvAccount_CellClick;
            this.dtgvTable.CellClick += this.dtgvTable_CellClick;
            this.dtgvCategory.CellClick += this.dtgvCategory_CellClick;

         
        }
        private void dtgvAccount_Click(object sender, EventArgs e)
        {

        }
        string idFood = ""; // nếu thai đồi cà 2 giá trị này thì sẽ mở nút Thêm
        string tenFood = "";
        private void frAdmin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet1.Food' table. You can move, or remove it, as needed.
       
            trangthaiClick();
            idFood = txbFoodID.Text;
            tenFood = txbFoodName.Text;
            tcBill.TabPages[0].BackColor = Color.PaleTurquoise;

            


        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = 0;
            if (numericUpDown1.Value == 1)
            {

                MessageBox.Show("Bạn không thể tăng lên 1 được, hảy tìm người có quyền hạng cao", "Hạn chế quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dtgvAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { }

        private void bntAdd_1_Table_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtNameTable.Text;
                string status = txtStatusTable.Text;

                if (TableDAO.Instance.InsertTable(name))
                {
                    MessageBox.Show("Thêm bàn thành công");
                    LoadTable();

                }
                else
                {
                    MessageBox.Show("Có lỗi khi thêm bàn");
                }
            }
            catch
            {
                MessageBox.Show("Bạn cần trỏ vào khoản trống thì mới thực thi Thêm !!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)// thai doi ten bàn
        {
            string name = txtNameTable.Text;
            //   string status = txtStatusTable.Text;
            txtStatusTable.Text = "Trống";
            int id = Convert.ToInt32(txtIdTable.Text);

            if (TableDAO.Instance.UpdateTable(name, id))
            {
                MessageBox.Show("Sửa bàn thành công");
                LoadTable();
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa bàn ăn");
            }
        }

        private void dtgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (txtIdTable.Text != string.Empty && txtStatusTable.Text != string.Empty && txtNameTable.Text != string.Empty)
            {
                bntAdd_1_Table.Enabled = false;
                button4.Enabled = true;
                btnUpdateTable.Enabled = true;
                btnDeleteTable.Enabled = true;
                btnAddTable.Enabled = false;
            }
            else
            {
                bntAdd_1_Table.Enabled = true;
                btnAddTable.Enabled = true;
                button4.Enabled = false;
                btnUpdateTable.Enabled = false;
                btnDeleteTable.Enabled = false;
            }
        }

        private void dtgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (txtIdCategory.Text != string.Empty && txtNameCategory.Text != string.Empty)
            {
                btnAddCategory.Enabled = false;
                bnt_update1_Category.Enabled = true;
                btnUpdateCategory.Enabled = true;
                btnDeleteCategory.Enabled = true;
                bnt_Add1_Category.Enabled = false;
            }
            else
            {
                btnAddCategory.Enabled = true;
                bnt_Add1_Category.Enabled = true;
                bnt_update1_Category.Enabled = false;
                btnUpdateCategory.Enabled = false;
                btnDeleteCategory.Enabled = false;
            }
        }

        private void bnt_Add1_Category_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtNameCategory.Text;

                if (FoodCategoryDAO.Instance.InsertFoodCategory(name))
                {
                    MessageBox.Show("Thêm danh mục thành công");
                    LoadListFoodCategory();
                }
                else
                {
                    MessageBox.Show("Có lỗi khi thêm danh mục");
                }
            }
            catch
            {
                MessageBox.Show("Bạn cần trỏ vào khoản trống thì mới thực thi Thêm !!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bnt_update1_Category_Click(object sender, EventArgs e)
        {
            string name = txtNameCategory.Text;
            int id = Convert.ToInt32(txtIdCategory.Text);

            if (FoodCategoryDAO.Instance.UpdateFoodCategory(name, id))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFoodCategory();
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }
        }
        Food_LinQDataContext conn;
        private void btnAddFood_gr_Click(object sender, EventArgs e)
        {
           try
           {
              
                   OpenFileDialog ope = new OpenFileDialog();
                   ope.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                   if (ope.ShowDialog() == DialogResult.Cancel)
                       return;
                   FileStream strem = new FileStream(ope.FileName, FileMode.Open);
                   IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(strem);
                   DataSet result = excelReader.AsDataSet();

                   conn = new Food_LinQDataContext();
                   foreach (DataTable tb in result.Tables)
                   {
                       foreach (DataRow dr in tb.Rows)
                       {
                           Food_linq addtable = new Food_linq()
                           {
                               id = Convert.ToInt32(dr[0]),
                               name = Convert.ToString(dr[1]),
                               idCategory = Convert.ToInt32(dr[2]),
                               price = Convert.ToDouble(dr[3])
                           };
                           conn.Food_linqs.InsertOnSubmit(addtable);
                       }
                   }
                     conn.SubmitChanges(); 
              
                excelReader.Close();
                strem.Close();
                MessageBox.Show("Đã Chuyễn Thành Công", "Thông Báo", MessageBoxButtons.OK);
            }
          catch
            {
                MessageBox.Show("Chuyễn Không Thành Công!! \n 1: Tắt Chương trình Excel \n 2: Coi lại mã ID (Thức ăn và Danh mục) trong file Excel ", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
        }

        private void dtgvFood_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { }

        private void txbFoodName_TextChanged(object sender, EventArgs e)
        {      }

        private void txbFoodName_Click(object sender, EventArgs e)
        {  }

    

        private void btnUpdate_MouseClick(object sender, MouseEventArgs e)
        {  }

        private void btnAddFood_MouseHover(object sender, EventArgs e)
        {    }
    
      
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tpBill_Click(object sender, EventArgs e)
        {

        }

        private void tcBill_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void nmFoodPrice_ValueChanged(object sender, EventArgs e)
        {

        }

       
    }
}


using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
namespace QuanLyQuanKaraoke//QuanLyCafe
{
    public partial class frManager : Form
    {
        private Account loginAccount;
    
        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }
        public frManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();

            LoadCategory();

            LoadComboboxTable(cbSwitchTable);
        }

        #region Method
        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinCáNhânToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food_DTO> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }
        Button btn;
        void LoadTable()
        {

            flbTable.Controls.Clear();

            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
             btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine;
                btn.ForeColor = Color.Red;

                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        btn.Image = QuanLyQuanKaraoke.Properties.Resources.table;
                        btn.TextImageRelation = TextImageRelation.ImageAboveText;

                        break;
                    default:
                        btn.BackColor = Color.Yellow;
                        btn.Image = QuanLyQuanKaraoke.Properties.Resources.dining;
                        btn.TextImageRelation = TextImageRelation.ImageAboveText;
                        break;
                }

                flbTable.Controls.Add(btn);

            }
        }
       
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<QuanLyCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (QuanLyCafe.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culu = new CultureInfo("vi-VN"); // dinh dang thành đ
            Thread.CurrentThread.CurrentCulture = culu;

            txbTotalPrice.Text = totalPrice.ToString("c");
           

        }
        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        #endregion


        #region Events

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frAccountProfile f = new frAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }
        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoànToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frAdmin f = new frAdmin();
            f.loginAccount = loginAccount;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            this.Hide();
            f.ShowDialog();
        }
        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {


        }

        #endregion

        private void cbCategory_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (nmFoodCount.Value != 0)
                {
                    Table table = lsvBill.Tag as Table;

                    if (table == null)
                    {
                        MessageBox.Show("Hãy chọn bàn");
                        return;
                    }

                    int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                    int foodID = (cbFood.SelectedItem as Food_DTO).ID;
                    int count = (int)nmFoodCount.Value;

                    if (idBill == -1)
                    {
                        BillDAO.Instance.InsertBill(table.ID);
                        BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
                    }
                    else
                    {
                        BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
                    }

                    ShowBill(table.ID);

                    LoadTable();
                    nmFoodCount.Value = 0;
                }
                else
                {
                    { MessageBox.Show("Bạn chưa chọn số lượng món ăn", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Stop); }
                }
            }
            catch
            {
                { MessageBox.Show("Lỗi, mong bạn thử lại sau", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Error); }
            }
        }
 
        private void btnCheckOut_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (txbTotalPrice.Text != "0,00 ₫")
                {
                    {
                        Table table = lsvBill.Tag as Table;

                        int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                        int discount = (int)numericUpDown1.Value;
                        double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);
                        double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;


                        if (idBill != -1)
                        {
                            if (setthucthi = false)
                            {
                                if (MessageBox.Show("Bạn có chắc thanh toán hóa đơn cho bàn này " + table.Name, "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                                {
                                    BillDAO.Instance.CheckOut(idBill, (float)finalTotalPrice, discount);
                                    ShowBill(table.ID);

                                    LoadTable();
                                    txttiengiam.Clear();
                                    
                                    numericUpDown1.Value = 1;
                                }

                            }
                            else
                            {
                                if (MessageBox.Show("Bạn có chắc thanh toán hóa đơn cho " + table.Name + "\n Bàn này có giảm giá với số tiền " + txttiengiam.Text, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    BillDAO.Instance.CheckOut(idBill, (float)finalTotalPrice, discount);
                                    ShowBill(table.ID);

                                    LoadTable();
                                    txttiengiam.Clear();
                                    txbTotalPrice.Clear();
                                    

                                }
                                else
                                {
                                    MessageBox.Show(" Thanh toán hóa đơn cho " + table.Name + "\n Bàn này có giảm giá với số tiền " + txttiengiam.Text + "\n Không thành công", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bàn bạn chọn chưa có thực dơn nên không thể thanh toán.!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop); 
                }
            }
            catch
            { MessageBox.Show("Bạn chưa chọn bàn đề thanh toán ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
}
        private void btnSwitchRoom_Click(object sender, EventArgs e)
        {
            try
            {
                int id1 = (lsvBill.Tag as Table).ID;

                int id2 = (cbSwitchTable.SelectedItem as Table).ID;
                if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    TableDAO.Instance.SwitchTable(id1, id2);

                    LoadTable();
                }
            }
            catch
            {
                MessageBox.Show("Bạn chưa chọn bàn đề chuyển ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void thôngTinTàiKhoànToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void SizeLastColumn(ListView lv)
        {
            int x = lv.Width / 15 == 0 ? 1 : lv.Width / 15;
            lv.Columns[0].Width = x * 5;
            lv.Columns[1].Width = x*3;
            lv.Columns[2].Width = x*3 ;
            lv.Columns[3].Width = x *5;
       
        }
        private void frManager_Load(object sender, EventArgs e)
        {
            SizeLastColumn(lsvBill);
   
        }

        private void nmFoodCount_ValueChanged(object sender, EventArgs e)
        {
          
        }
        double tientong = 0;
        private void bntDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                bntDiscount.BackColor = Color.Yellow;
                tientong = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);
                numericUpDown1.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Bạn chưa chọn bàn đề thanh toán ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void lsvBill_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCheckOut_MouseHover(object sender, EventArgs e)
        {
            btnCheckOut.BackColor = Color.MistyRose;
        }

        private void btnAddFood_MouseHover(object sender, EventArgs e)
        {
            btnAddFood.BackColor = Color.MistyRose;
        }

        private void btnAddFood_MouseLeave(object sender, EventArgs e)
        {
            btnAddFood.BackColor = Color.White;
        }

        private void btnCheckOut_MouseLeave(object sender, EventArgs e)
        {
            btnCheckOut.BackColor = Color.White;
        }
        bool setthucthi = false;
        
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

            setthucthi = true;
       

                Table table = lsvBill.Tag as Table;

                int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                int discount = (int)numericUpDown1.Value;
                double totalPrice = tientong;

                CultureInfo culu = new CultureInfo("vi-VN"); // dinh dang thành đ
                Thread.CurrentThread.CurrentCulture = culu;

                txttiengiam.Text = Convert.ToString((totalPrice / 100) * discount);
              double tiengiam = Convert.ToDouble(txttiengiam.Text);
          txbTotalPrice.Text = Convert.ToString(tientong - tiengiam);
          

            
         

            
        }

        private void txbTotalPrice_TextChanged(object sender, EventArgs e)
        {   }

        private void cbSwitchTable_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lsvBill_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lsvBill.Columns[e.ColumnIndex].Width;
        }

        private void lsvBill_Resize_1(object sender, EventArgs e)
        {
            SizeLastColumn((ListView)sender);

        }
      
        


    }
}


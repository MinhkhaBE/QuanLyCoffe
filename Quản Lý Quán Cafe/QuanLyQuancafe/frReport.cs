using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanKaraoke
{
    public partial class frReport : Form
    {
        string ckt = @"Data Source=DESKTOP-CI36P6F\SQLEXPRESS;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
        SqlConnection ketnoi;
        SqlCommand thucthi;
        DataTable dt = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        public frReport()
        {
            InitializeComponent();
        }
        public void hienthi()
        {
            CrystalReport1 rp = new CrystalReport1();
            thucthi = ketnoi.CreateCommand();
            thucthi.CommandText = @"SELECT t.name, b.totalPrice, DateCheckIn, DateCheckOut
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE b.status = 1
	AND t.id = b.idTable";
            adapter.SelectCommand = thucthi;

            dt = new DataTable();
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = thucthi;
            adapter.Fill(dt);
            rp.SetDataSource(dt);

            crystalReportViewer1.ReportSource = rp;
        }

        private void frReport_Load(object sender, EventArgs e)
        {
            ketnoi = new SqlConnection(ckt);
            ketnoi.Open();
            hienthi();
        }
    }
}

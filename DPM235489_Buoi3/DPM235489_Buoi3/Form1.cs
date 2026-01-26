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

namespace DPM235489_Buoi3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DataSet ds = new DataSet("dsQLNV");
        SqlDataAdapter daChucVu;
        SqlDataAdapter daNhanVien;
        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MT0315\SQLEXPRESS; Initial Catalog=QLNV;Integrated Security=True";
            // Dữ liệu combobox Chức vụ
            string sQueryChucVu = @"select * from chucvu";
            daChucVu = new SqlDataAdapter(sQueryChucVu, conn);
            daChucVu.Fill(ds, "tblChucVu");
            cboChucVu.DataSource = ds.Tables["tblChucVu"];
            cboChucVu.DisplayMember = "tencv";
            cboChucVu.ValueMember = "macv";
            // Dữ liệu datagrid Danh sách nhân viên
            string sQueryNhanVien = @"select n.*, c.tencv from nhanvien n, chucvu c where
n.macv=c.macv";
            daNhanVien = new SqlDataAdapter(sQueryNhanVien, conn);
            daNhanVien.Fill(ds, "tblDSNhanVien");
            dgDSNhanVien.DataSource = ds.Tables["tblDSNhanVien"];
            dgDSNhanVien.Columns["manv"].HeaderText = "Mã số";
            dgDSNhanVien.Columns["manv"].Width = 60;
            // ... đặt tiêu đề tiếng Việt, định độ rộng cho các trường còn lại
            dgDSNhanVien.Columns["macv"].Visible = false;
            string sThemNV = @"insert into nhanvien values(@MaNV, @HoLot, @TenNV, @Phai,
@NgaySinh, @MaCV)";
            SqlCommand cmThemNV = new SqlCommand(sThemNV, conn);
            cmThemNV.Parameters.Add("@MaNV", SqlDbType.NVarChar, 5, "manv");
            cmThemNV.Parameters.Add("@HoLot", SqlDbType.NVarChar, 50, "holot");
            cmThemNV.Parameters.Add("@TenNV", SqlDbType.NVarChar, 10, "tennv");
            cmThemNV.Parameters.Add("@Phai", SqlDbType.NVarChar, 3, "phai");
            cmThemNV.Parameters.Add("@NgaySinh", SqlDbType.SmallDateTime, 10,
            "ngaysinh");
            cmThemNV.Parameters.Add("@MaCV", SqlDbType.NVarChar, 5, "macv");
            daNhanVien.InsertCommand = cmThemNV;
        }

        private void dgDSNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow dr = dgDSNhanVien.SelectedRows[0];
            txtMaNV.Text = dr.Cells["manv"].Value.ToString();
            txtHoLot.Text = dr.Cells["holot"].Value.ToString();
            txtTen.Text = dr.Cells["tennv"].Value.ToString();
            if (dr.Cells["phai"].Value.ToString() == "Nam")
            {
                radNam.Checked = true;
            }
            else
            {
                radNu.Checked = true;
            }
            dtpNgaySinh.Text = dr.Cells["ngaysinh"].Value.ToString();
            cboChucVu.SelectedValue = dr.Cells["macv"].Value.ToString();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text) ||  string.IsNullOrWhiteSpace(txtHoLot.Text) || string.IsNullOrWhiteSpace(txtTen.Text) ||   cboChucVu.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            foreach (DataRow dr in ds.Tables["tblDSNhanVien"].Rows)
            {
                if (dr["manv"].ToString() == txtMaNV.Text)
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại!");
                    return;
                }
            }
            DataRow row = ds.Tables["tblDSNhanVien"].NewRow();
            row["manv"] = txtMaNV.Text;
            row["holot"] = txtHoLot.Text;
            row["tennv"] = txtTen.Text;
            if (radNu.Checked == true)
            {
                row["phai"] = "Nữ";
            }
            else
            {
                row["phai"] = "Nam";
            }
            row["ngaysinh"] = dtpNgaySinh.Text;
            row["macv"] = cboChucVu.SelectedValue;
            ds.Tables["tblDSNhanVien"].Rows.Add(row);
            DataTable dtCV = ds.Tables["tblChucVu"];

            DataRow[] r = dtCV.Select(
                "macv = '" + cboChucVu.SelectedValue.ToString() + "'"
            );

            if (r.Length > 0)
            {
                row["tencv"] = r[0]["tencv"].ToString();
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ProjectVisproFD
{
    public partial class FrmReg : Form
    {
        private MySqlConnection koneksi;
        private MySqlDataAdapter adapter;
        private MySqlCommand perintah;
        private DataSet ds = new DataSet();
        private string alamat, query;

        public FrmReg()
        {
            alamat = "server=localhost; database=db_filkomday; username=root; password=;";
            koneksi = new MySqlConnection(alamat);
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            txtRegis.Clear();
            txtNama.Clear();
            cbBaju.SelectedIndex = -1;
            cbMakanan.SelectedIndex = -1;

            txtRegis.Enabled = true;
            txtRegis.Focus();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSearch.Enabled = true;
            btnClear.Enabled = false;

            FrmReg_Load(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                FrmReg_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmReg_Load(object sender, EventArgs e)
        {
            try
            {
                koneksi.Open();
                query = string.Format("select * from tbl_pengguna");
                perintah = new MySqlCommand(query, koneksi);
                adapter = new MySqlDataAdapter(perintah);
                perintah.ExecuteNonQuery();
                ds.Clear();
                adapter.Fill(ds);
                koneksi.Close();

                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[0].Width = 100;
                dataGridView1.Columns[0].HeaderText = "No. Regis";
                dataGridView1.Columns[1].Width = 150;
                dataGridView1.Columns[1].HeaderText = "Nama";
                dataGridView1.Columns[2].Width = 120;
                dataGridView1.Columns[2].HeaderText = "Size Baju";
                dataGridView1.Columns[3].Width = 120;
                dataGridView1.Columns[3].HeaderText = "Makanan";

                txtRegis.Clear();
                txtNama.Clear();
                cbBaju.SelectedIndex = -1;         // <-- Added to clear Size Baju
                cbMakanan.SelectedIndex = -1;      // <-- Added to clear Makanan
                txtRegis.Focus();

                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnClear.Enabled = false;
                btnSave.Enabled = true;
                btnSearch.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRegis.Text != "" && txtNama.Text != "")
                {
                    string folderPath = Path.Combine(Application.StartupPath, "C:\\Users\\DIGITAL MARKETING\\source\\repos\\FinalProjectFD");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                    query = string.Format("insert into tbl_pengguna  values ('{0}','{1}','{2}','{3}');", txtRegis.Text, txtNama.Text, cbBaju.Text, cbMakanan.Text, fileName);

                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    adapter = new MySqlDataAdapter(perintah);
                    int res = perintah.ExecuteNonQuery();
                    koneksi.Close();

                    if (res == 1)
                    {
                        MessageBox.Show("Insert Data Successfully");
                        FrmReg_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Insert Data Failed");
                    }
                }
                else
                {
                    MessageBox.Show("Missing Required Data");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ds.Clear();
                string query = "SELECT * FROM tbl_pengguna WHERE 1=1";
                MySqlCommand perintah = new MySqlCommand();
                perintah.Connection = koneksi;

                if (!string.IsNullOrWhiteSpace(txtRegis.Text))
                {
                    query += " AND regis = @regis";
                    perintah.Parameters.AddWithValue("@regis", txtRegis.Text);
                }

                if (!string.IsNullOrWhiteSpace(txtNama.Text))
                {
                    query += " AND nama = @nama";
                    perintah.Parameters.AddWithValue("@nama", txtNama.Text);
                }

                if (!string.IsNullOrWhiteSpace(cbBaju.Text))
                {
                    query += " AND size = @size";
                    perintah.Parameters.AddWithValue("@size", cbBaju.Text);
                }

                if (!string.IsNullOrWhiteSpace(cbMakanan.Text))
                {
                    query += " AND food = @food";
                    perintah.Parameters.AddWithValue("@food", cbMakanan.Text);
                }

                perintah.CommandText = query;

                koneksi.Open();
                adapter = new MySqlDataAdapter(perintah);
                adapter.Fill(ds);
                koneksi.Close();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow kolom = ds.Tables[0].Rows[0];
                    txtRegis.Text = kolom["regis"].ToString();
                    txtNama.Text = kolom["nama"].ToString();
                    cbBaju.Text = kolom["size"].ToString();
                    cbMakanan.Text = kolom["food"].ToString();

                    txtRegis.Enabled = true;
                    dataGridView1.DataSource = ds.Tables[0];
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSearch.Enabled = true;
                    btnClear.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Data Tidak Ada !!");
                    btnClear.PerformClick();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmMain frmMain = new FrmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRegis.Text != "" && txtNama.Text != "")
                {
                    string folderPath = Path.Combine(Application.StartupPath, "C:\\Users\\DIGITAL MARKETING\\source\\repos\\FinalProjectFD");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    query = string.Format("UPDATE tbl_pengguna SET nama = '{0}', size = '{1}', food = '{2}' WHERE regis = '{3}'", txtNama.Text, cbBaju.Text, cbMakanan.Text, txtRegis.Text);

                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    adapter = new MySqlDataAdapter(perintah);
                    int res = perintah.ExecuteNonQuery();
                    koneksi.Close();

                    if (res == 1)
                    {
                        MessageBox.Show("Update Data Suksess ...");
                        FrmReg_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Gagal Update Data . . . ");
                    }
                }
                else
                {
                    MessageBox.Show("Data Tidak lengkap !!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRegis.Text != "")
                {
                    if (MessageBox.Show("Anda Yakin Menghapus Data Ini ??", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        query = string.Format("Delete from tbl_pengguna where regis = '{0}'", txtRegis.Text);
                        ds.Clear();
                        koneksi.Open();
                        perintah = new MySqlCommand(query, koneksi);
                        adapter = new MySqlDataAdapter(perintah);
                        int res = perintah.ExecuteNonQuery();
                        koneksi.Close();

                        if (res == 1)
                        {
                            MessageBox.Show("Delete Data Suksess ...");
                        }
                        else
                        {
                            MessageBox.Show("Gagal Delete data");
                        }
                    }
                    FrmReg_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Data Yang Anda Pilih Tidak Ada !!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                FrmReg_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

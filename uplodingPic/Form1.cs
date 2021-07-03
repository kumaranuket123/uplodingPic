using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uplodingPic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            // TODO: This line of code loads data into the 'databaseDataSet.Pictures' table. You can move, or remove it, as needed.
           // this.picturesTableAdapter.Fill(this.databaseDataSet.Pictures);

        }
        public void Insert(string filename, byte[] image)
        {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["uplodingPic.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                using(SqlCommand cmd=new SqlCommand("Insert into pictures(filename, image) values(@filename , @image)", cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@filename", txtFileName.Text);
                    cmd.Parameters.AddWithValue("@image", image);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LoadData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["uplodingPic.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
               using(DataTable dt = new DataTable("Pictures"))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("select *from pictures", cn);
                    adapter.Fill(dt);
                    dataGridView.DataSource = dt;
                }
            }
        }

        byte[] convertImageToBytes(Image img)
        {
            using(MemoryStream ms= new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public Image convertByteArrayToImage(byte[] data)
        {
            using(MemoryStream ms= new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog() { Filter= "Image files(*.jpg;*jpeg)|*.jpg;*.jpeg", Multiselect= false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox.Image = Image.FromFile(ofd.FileName);
                    txtFileName.Text = ofd.FileName;
                    Insert(txtFileName.Text, convertImageToBytes(pictureBox.Image));
                    LoadData();
                }
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dt = dataGridView.DataSource as DataTable;
            if(dt != null)
            {
                DataRow row = dt.Rows[e.RowIndex];
                pictureBox.Image= convertByteArrayToImage((byte[])row["Image"]);
            }
        }
    }
}

using Common;
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

namespace FormRecognizerWinform
{
    public partial class frmImageParser : Form
    {
        public frmImageParser()
        {
            InitializeComponent();
            pictureBox1.Visible = false;
            dgvResult.Visible = false;
            lblMessage.Visible = false;                        
        }               
        private void btnViewImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Browse Image Files";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    txtBLOBUrl.Text = openFileDialog.FileName;
                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        var fileContent = reader.ReadToEnd();
                    }
                }
            }
            lblMessage.Visible = true;
            lblMessage.Text = "Uploading Image..";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.ImageLocation = ConfigurationConstants.imgUrl;
            pictureBox1.LoadCompleted += PictureBox1_LoadCompleted;
            pictureBox1.Visible = true;                        
        }

        private void PictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lblMessage.Text = "Analyzing Image..";
            GetFormRecognizerResult();
        }
        private async void GetFormRecognizerResult()
        {
            var lstResultDTO = await FormRecognizeAnalyzer.PerformFormRecognization();
            dgvResult.DataSource = lstResultDTO;
            dgvResult.Visible = true;
            lblMessage.Text = "Analysis Report Prepared";
        }
        private void frmImageParser_Load(object sender, EventArgs e)
        {

        }
    }
}

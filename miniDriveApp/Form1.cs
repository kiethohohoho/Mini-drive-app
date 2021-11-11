using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using miniDriveApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miniDriveApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ok");
        }
        private FileList FolderRoots { get; set; }
        public DriveService DriveService { get; set; }


        //private void button4_Click(object sender, EventArgs e)
        //{

        //    string selectedPath;//Biến chứa đường dẫn đây
        //    var t = new Thread((ThreadStart)(() => {
        //        FolderBrowserDialog fbd = new FolderBrowserDialog();
        //        fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
        //        fbd.ShowNewFolderButton = true;
        //        if (fbd.ShowDialog() == DialogResult.OK)
        //        {
        //            selectedPath = fbd.SelectedPath;
        //            MessageBox.Show(selectedPath);
        //            return;
        //        }   

        //        selectedPath = fbd.SelectedPath;
        //    }));

        //    t.SetApartmentState(ApartmentState.STA);
        //    t.Start();
        //    t.Join();

        //}
        private string FormatUploadInfo = "File đã tải: {0}/{1}\nđang tải lên:{2}";
        private void SetUploadInfo(int index, int amount, string pathfile)
        {
            lbUploadInfo.Text = string.Format(FormatUploadInfo, index, amount, pathfile);
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileNames.Length < 1)
                {
                    return;
                }
                if (comboBox1.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui long chon folder!");
                    return;
                }
                var IdFolder = comboBox1.SelectedValue.ToString();
                comboBox1.Enabled = false;
                int index = 0;
                foreach (var pathFile in dialog.FileNames)
                {
                    SetUploadInfo(index, dialog.FileNames.Length, pathFile);

                    try
                    {
                        var file = await UploadFileAsync(IdFolder, pathFile);
                        ListData.Insert(0, file);
                        dataGridView2.DataSource = null;
                        dataGridView2.DataSource = ListData;
                    }
                    catch
                    {
                        var result = MessageBox.Show($"{pathFile} tai len loi!\ntiep tuc tai len?",
                              "Thong bao"
                              , MessageBoxButtons.YesNo
                              , MessageBoxIcon.Error);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    SetUploadInfo(++index, dialog.FileNames.Length, pathFile);
                }
                comboBox1.Enabled = true;
            }
        }


        public async Task<Google.Apis.Drive.v3.Data.File> UploadFileAsync(string IdFolder, string pathFile)
        {
            return await DriveService.UploadFileAsync(pathFile, IdFolder);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }



        private async void Form1_Load(object sender, EventArgs e)
        {
            DriveService = await Program.GoogleService.CreateDriveSevericeAsync();
            FolderRoots = DriveService.GetFolder();
            comboBox1.DisplayMember = nameof(Google.Apis.Drive.v3.Data.File.Name);
            comboBox1.ValueMember = nameof(Google.Apis.Drive.v3.Data.File.Id);
            comboBox1.DataSource = FolderRoots.Files;
        }

        public IList<Google.Apis.Drive.v3.Data.File> GetFileFolder(string IdFolder)
        {
            IEnumerable<Google.Apis.Drive.v3.Data.File> LstFile = new List<Google.Apis.Drive.v3.Data.File>();
            var FileList = DriveService.GetFiles(IdFolder);
            LstFile = LstFile.Concat(FileList.Files);

            while (!string.IsNullOrEmpty(FileList.NextPageToken))
            {
                FileList = DriveService.GetFiles(IdFolder, FileList.NextPageToken);
                LstFile = LstFile.Concat(FileList.Files);
            }

            return LstFile.ToList();
        }
        private IList<Google.Apis.Drive.v3.Data.File> ListData { get; set; }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;
            if (combobox.SelectedIndex < 0)
            {
                return;
            }

            var IdFolder = combobox.SelectedValue.ToString();
            ListData = GetFileFolder(IdFolder);
            dataGridView2.DataSource = ListData;
        }
    }
}

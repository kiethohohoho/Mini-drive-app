using Google.Apis.Drive.v3;
using miniDriveApp.Services;
using System;
using System.Windows.Forms;

namespace miniDriveApp
{
    static class Program
    {
        public static GoogleDriveService GoogleService { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GoogleService = new GoogleDriveService();
            GoogleService.AuthorizeAsync(DriveService.Scope.Drive).Wait();
            //var driveService =await drive.CreateDriveSevericeAsync(identity);
            //var tests = await drive.UploadFileAsync(@"Desktop\y_tuong_chong_dich.jpg", "");
            //var test = driveService.GetFolder();
            //var test2 = driveService.GetFiles(test.Files[0].Id);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

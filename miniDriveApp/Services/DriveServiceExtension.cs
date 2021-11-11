using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Threading.Tasks;

namespace miniDriveApp.Services
{
    public static class DriveServiceExtension
    {
        public static DriveList GetDrives(this DriveService driveService)
        {
            var request = driveService.Drives.List();
            return request.Execute();
        }
        public static FileList GetFolder(this DriveService driveService, string FolderId = null, string nextPage = null)
        {
            var request = driveService.Files.List();
            request.Q = "mimeType = 'application/vnd.google-apps.folder'";
            request.Fields = "*";
            if (!string.IsNullOrEmpty(nextPage))
            {
                request.PageToken = nextPage;
            }
            if (!string.IsNullOrEmpty(FolderId))
            {
                request.Q += $"and '{FolderId}' in parents";
            }
            else
            {
                request.Q += $"and 'root' in parents";
            }
            return request.Execute();
        }
        public static FileList GetFiles(this DriveService driveService, string FolderId = null, string nextPage = null)
        {
            var request = driveService.Files.List();
            request.Q = "mimeType != 'application/vnd.google-apps.folder'";
            request.Fields = "*";
            if (!string.IsNullOrEmpty(nextPage))
            {
                request.PageToken = nextPage;
            }
            if (!string.IsNullOrEmpty(FolderId))
            {
                request.Q += $"and '{FolderId}' in parents";
            }
            else
            {
                request.Q += $"and 'root' in parents";
            }
            return request.Execute();
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static async Task<Google.Apis.Drive.v3.Data.File> UploadFileAsync(this DriveService driveService, string Pathfile, string FolderId = null)
        {
            if (!System.IO.File.Exists(Pathfile))
            {
                throw new Exception("File not is exist!");
            }
            //khoi tao
            var fileName = System.IO.Path.GetFileName(Pathfile);
            var metaData = new Google.Apis.Drive.v3.Data.File();
            metaData.Name = fileName;
            if (!string.IsNullOrEmpty(FolderId))
            {
                metaData.Parents = new string[] { FolderId };
            }
            //load file
            var stream = System.IO.File.OpenRead(Pathfile);
            //goi service
            var contentType = GetMimeType(fileName);
            var requestFile = driveService.Files.Create(metaData, stream, contentType);
            requestFile.Fields = "*";

            var res = await requestFile.UploadAsync();
            if (res.Status == Google.Apis.Upload.UploadStatus.Failed)
            {
                throw new Exception("Upload fail!");
            }
            return requestFile.ResponseBody;
        }
    }
}

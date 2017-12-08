using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;
using System.Data;
using System.IO;

namespace Common.utils
{
    public class WinScpHelper
    {
        private string FtpUrl { get; set; }
        private string FtpHost { get; set; }
        private int PortNumber { get; set; }
        private string FtpUsername { get; set; }
        private string FtppPassword { get; set; }
        private string FtpSSHFingerPrint { get; set; }
        private string SshPrivateKeyPath { get; set; }
        private SessionOptions SessionOptions { get; set; }
        private string LogPath { get; set; }


        public WinScpHelper(string host, string username, string password, string fingerprint, string pkeypath, string logpath)
        {
            this.FtpHost = host;
            this.FtpUsername = username;
            this.FtppPassword = password;
            this.FtpSSHFingerPrint = fingerprint;
            this.SshPrivateKeyPath = pkeypath;
            this.PortNumber = 22;
            this.SessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = this.FtpHost,
                UserName = this.FtpUsername,
                Password = this.FtppPassword,
                PortNumber = this.PortNumber,
                SshHostKeyFingerprint = this.FtpSSHFingerPrint,
                SshPrivateKeyPath = this.SshPrivateKeyPath,

            };
            this.LogPath = logpath;

        }

        public List<string> GetFileList(string ftpdir)
        {
            List<string> files = new List<string>();
            try
            {
                using (Session session = new Session())
                {

                    session.SessionLogPath = this.LogPath;
                    session.Open(this.SessionOptions);
                    RemoteDirectoryInfo directory = session.ListDirectory(ftpdir);
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (fileInfo.Name != "..")
                        {
                            files.Add(fileInfo.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message + ex.StackTrace;
            }
            return files;

        }

        public string GetLastestFileList(string ftpdir, string fileFilter)
        {
            string fileName = string.Empty;
            using (Session session = new Session())
            {
                session.Open(this.SessionOptions);
                //RemoteDirectoryInfo directory = session.ListDirectory(ftpdir);
                var q = session.EnumerateRemoteFiles(ftpdir, "", EnumerationOptions.None);
                fileName = q.Where(o=>o.Name.StartsWith(fileFilter)).OrderByDescending(o => o.LastWriteTime).FirstOrDefault().ToString();
                //DateTime latesttime = DateTime.MinValue;
                //foreach (RemoteFileInfo fileInfo in directory.Files)
                //{
                //    if (fileInfo.Name != ".." && fileInfo.Name.StartsWith(fileFilter))
                //    {
                //        if (fileInfo.LastWriteTime > latesttime){
                //            latesttime = fileInfo.LastWriteTime;
                //            fileName = fileInfo.Name;
                //        }
                //    }
                //}
            }
            return fileName;
        }


        public bool Download(string ftpdir, string filename, string localdir)
        {
            bool success = false;
            try
            {
                using (Session session = new Session())
                {

                    session.SessionLogPath = this.LogPath;
                    session.Open(this.SessionOptions);
                    RemoteDirectoryInfo directory = session.ListDirectory(ftpdir);
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (String.IsNullOrEmpty(filename) || (!String.IsNullOrEmpty(filename) && filename == fileInfo.Name))
                        {
                            if (fileInfo.Name != "..")
                            {
                                TransferOptions transferOptions = new TransferOptions();
                                transferOptions.TransferMode = TransferMode.Binary;
                                transferOptions.FilePermissions = null;
                                transferOptions.PreserveTimestamp = false;
                                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
                                TransferOperationResult transferResult;
                                transferResult = session.GetFiles(string.Format("{0}/{1}", ftpdir, fileInfo.Name), localdir, false, transferOptions);
                                transferResult.Check();
                            }
                        }
                    }
                    success = true;
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message + ex.StackTrace;
                success = false;
            }
            return success;
        }

        public bool UploadFile(string sourceFilePath, string descFilepath)
        {
            bool success = false;
            try
            {

                using (Session session = new Session())
                {
                    // Connect
                    session.SessionLogPath = this.LogPath;
                    session.Open(this.SessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    string[] dirs = Path.GetDirectoryName(descFilepath).Split('/').Where(o => o.ToString().Length > 0).ToArray();
                    string dirpath = "/";
                    for (int i = 0; i < dirs.Count(); i++)
                    {
                        dirpath += dirs[i] + '/';
                        if (!session.FileExists(dirpath)) session.CreateDirectory(dirpath);
                    }
                    TransferOperationResult transferResult = session.PutFiles(sourceFilePath, Path.GetFileName(descFilepath), false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        success = true;
                    }
                }

                // Delete the file after uploading
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message + ex.StackTrace;
            }
            return success;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSCP;
using System.Data;
using System.IO;

namespace Common.Utils
{
    public class WinScpHelper
    {
        public string FtpUrl {get;set;}
        public string FtpHost { get; set; }
        public int PortNumber { get; set; }
        public string FtpUsername { get; set; }
        public string FtppPassword { get; set; }
        public string FtpSSHFingerPrint { get; set; }
        public string SshPrivateKeyPath { get; set; } 


        public WinScpHelper(string host,string username,string fingerprint,string pkeypath)
        {
            this.FtpHost = host;
            this.FtpUsername = username;
            this.FtppPassword = string.Empty;
            this.FtpSSHFingerPrint = fingerprint;
            this.SshPrivateKeyPath = pkeypath;
            this.PortNumber = 21;
            
        }


        public  void Download(string ftpdir , string localdir)
        {
            try
            {              
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = this.FtpHost,
                    UserName = this.FtpUsername,
                    // Password = this.FtppPassword,
                    PortNumber = this.PortNumber,
                    SshHostKeyFingerprint = this.FtpSSHFingerPrint,
                    SshPrivateKeyPath = this.SshPrivateKeyPath,
                };

                using (Session session = new Session())
                {
                    session.SessionLogPath = "";
                    session.Open(sessionOptions);
                    RemoteDirectoryInfo directory = session.ListDirectory(ftpdir);
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if(fileInfo.Name != ".."){
                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Binary;
                            transferOptions.FilePermissions = null;
                            transferOptions.PreserveTimestamp = false;
                            transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
                            TransferOperationResult transferResult;
                            transferResult = session.GetFiles(ftpdir + fileInfo.Name, localdir, false, transferOptions);
                            transferResult.Check();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message + ex.StackTrace;
            }
        }


        public bool UploadFile(string sourcefilepath, string ftpdir)
        {
            bool success = false;
            try
            {
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = this.FtpHost,
                    UserName = this.FtpUsername,
                    // Password = this.FtppPassword,
                    PortNumber = this.PortNumber,
                    SshHostKeyFingerprint = this.FtpSSHFingerPrint,
                    SshPrivateKeyPath = this.SshPrivateKeyPath,
                };

                string ftpfullpath = ftpdir + Path.GetFileName(sourcefilepath);

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult = session.PutFiles(sourcefilepath, ftpfullpath, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        success = true;
                    }
                }

                // Delete the file after uploading
                if (File.Exists(sourcefilepath))
                {
                    File.Delete(sourcefilepath);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.Utils
{
    public class RemoteConnectorHelper
    {
        public static bool Connect(string remoteHost, string userName, string passWord)
        {
            try
            {
                bool Flag = string.IsNullOrEmpty(Connect(remoteHost, userName, passWord, 2));
                return Flag;
            }
            catch
            {
                return false;
            }

            //bool Flag = true;
            //Process proc = new Process();
            //proc.StartInfo.FileName = "cmd.exe";
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.RedirectStandardInput = true;
            //proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.RedirectStandardError = true;
            //proc.StartInfo.CreateNoWindow = true;

            //if (remoteHost.StartsWith("\\\\"))
            //{
            //    remoteHost = remoteHost.Remove(0, 2);
            //}
            //string deleteCommand = string.Format(@"net  use  \\{0} /del>NUL", remoteHost);
            //string netUseCommand = string.Format(@"net  use  \\{0} {1} /user:{2}>NUL", remoteHost, passWord, userName);
            //string exitCommand = "exit";
            //try
            //{
            //    int retry = 2;
            //    while (retry-- > 0 )
            //    {
            //        proc.Start();
            //        proc.StandardInput.WriteLine(netUseCommand);
            //        proc.StandardInput.WriteLine(exitCommand);
            //        while (proc.HasExited == false)
            //        {
            //            proc.WaitForExit(1000);
            //        }
            //        string errormsg = proc.StandardError.ReadToEnd();
            //        Flag = string.IsNullOrEmpty(errormsg);
            //        if (!Flag)
            //        {
            //            //delete current usage to avoid error
            //            proc.Start();
            //            proc.StandardInput.WriteLine(deleteCommand);
            //            continue;
            //        }
            //        else
            //        {
            //            proc.StandardError.Close();
            //            break;
            //        }
            //    }
            //}
            //catch// (Exception ex)
            //{
            //    Flag = false;
            //}
            //finally
            //{
            //    proc.Close();
            //    proc.Dispose();
            //}
            //return Flag;
        }


        public static string Connect(string remoteHost, string userName, string passWord, int retry)
        {
            string errorMsg = null;
            bool Flag = true;
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            if (remoteHost.StartsWith("\\\\"))
            {
                remoteHost = remoteHost.Remove(0, 2);
            }
            string deleteCommand = string.Format(@"net  use  \\{0} /del>NUL", remoteHost);
            string netUseCommand = string.Format(@"net  use  \\{0} {1} /user:{2}>NUL", remoteHost, passWord, userName);
            string exitCommand = "exit";
            try
            {
                if (retry < 1) retry = 1;

                while (retry-- > 0)
                {
                    proc.Start();
                    proc.StandardInput.WriteLine(netUseCommand);
                    proc.StandardInput.WriteLine(exitCommand);
                    while (proc.HasExited == false)
                    {
                        proc.WaitForExit(1000);
                    }
                    errorMsg = proc.StandardError.ReadToEnd();
                    proc.StandardError.Close();
                    Flag = string.IsNullOrEmpty(errorMsg);
                    if (!Flag)
                    {
                        //delete current usage to avoid error
                        proc.Start();
                        proc.StandardInput.WriteLine(deleteCommand);
                        proc.StandardInput.WriteLine(exitCommand);
                        while (proc.HasExited == false)
                        {
                            proc.WaitForExit(1000);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return errorMsg;
        }
    }
}

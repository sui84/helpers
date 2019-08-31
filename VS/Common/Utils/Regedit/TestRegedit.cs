using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace Common.Utils.Regedit
{
    public class TestRegedit
    {
        static void Main(string[] args)
        {
            try
            {
                RegistryKey driverKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows",true);
                driverKey.DeleteSubKeyTree("ShellNoRoam");
                return;
          
                string keystr = @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRULegacy";
                RegHelper rh = new RegHelper();
                rh.IsRegeditKeyExit(keystr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            Thread.Sleep(999999999);
        }
    }
}

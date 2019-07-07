using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils.Log;
using log4net;
using System.Threading;
using Common.Utils.Ntfs;
using System.IO;

namespace Common.Utils.Utfs
{
    public class TestNtfs
    {
        static void Main(string[] args)
        {
            try
            {
                DriveInfo di = new DriveInfo(@"E:\");
               // List<UsnEntry> ues = FileQueryEngine.GetAllFiles(di);
              // List<FileAndDirectoryEntry> ues = FileQueryEngine.GetAllFileEntrys(di);
                //var usnOperator = new UsnOperator(di);
              //  usnOperator.GetUsnJournal();


               // List<string> files = new MFTScanner().EnumerateFiles("E:").ToList();
                UsnJournalHelper ujhelper = new UsnJournalHelper("E:");
               ujhelper.ViewUsnChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);     
            }
            Thread.Sleep(999999999);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.Utils.Utfs;
using System.Threading;
using Common.Utils.ConvertType;
using System.Runtime.InteropServices.ComTypes;
using Common.Utils.Log;

namespace Common.Utils.Ntfs
{
    public class FileChange
    {   
        public ulong FileReferenceNumber { get;  set; }
        public string FileName { get;  set; }
        public string FilePath { get;  set; }
        public long Usn { get;  set; }
        public string ChangeType { get;  set; }
        public DateTime  ChangeDate { get;  set; }
        public bool IsFile { get; set; }
    }

    public class UsnJournalHelper
    {
        #region Properties

        private NtfsUsnJournal _usnJournal = null;
        public NtfsUsnJournal Journal
        {
            get { return _usnJournal; }
        }

        private Win32Api.USN_JOURNAL_DATA _usnCurrentJournalState;
        private DriveInfo _volume;

        #endregion

        public  UsnJournalHelper(string driver)
        {
            DriveInfo[] volumes = DriveInfo.GetDrives();
            DriveInfo volume = volumes.Where(o => o.Name.StartsWith(driver)).FirstOrDefault();
            _usnJournal = new NtfsUsnJournal(volume);
            _volume = volume;
            NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.GetUsnJournalState(ref _usnCurrentJournalState);
        }

         public void GetVolumes()
        {
            DriveInfo[] volumes = DriveInfo.GetDrives();
            List<string> drives = new List<string>();
            foreach (DriveInfo di in volumes)
            {
                if (di.IsReady && 0 == string.Compare(di.DriveFormat, "ntfs", true))
                {
                    drives.Add(di.Name);
                }
            }
        }

         public string QueryUsnJournal()
         {
             string content = string.Empty;
             NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.GetUsnJournalState(ref _usnCurrentJournalState);
            //  FunctionElapsedTime.Content = string.Format("Query->{0} elapsed time {1}(ms)",
            //    "GetUsnJournalState()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

             if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
             {
                 content = FormatUsnJournalState(_usnCurrentJournalState);
             }
             else
             {
                content =  rtn.ToString();
             }
             return content;
         }

         private string FormatUsnJournalState(Win32Api.USN_JOURNAL_DATA _usnCurrentJournalState)
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine(string.Format("Driver : {0}", _volume.Name));
             sb.AppendLine(string.Format("Journal ID: {0}", _usnCurrentJournalState.UsnJournalID.ToString("X")));
             sb.AppendLine(string.Format(" First USN: {0}", _usnCurrentJournalState.FirstUsn.ToString("X")));
             sb.AppendLine(string.Format("  Next USN: {0}", _usnCurrentJournalState.NextUsn.ToString("X")));
             sb.AppendLine();
             sb.AppendLine(string.Format("Lowest Valid USN: {0}", _usnCurrentJournalState.LowestValidUsn.ToString("X")));
             sb.AppendLine(string.Format("         Max USN: {0}", _usnCurrentJournalState.MaxUsn.ToString("X")));
             sb.AppendLine(string.Format("        Max Size: {0}", _usnCurrentJournalState.MaximumSize.ToString("X")));
             sb.AppendLine(string.Format("Allocation Delta: {0}", _usnCurrentJournalState.AllocationDelta.ToString("X")));
             return sb.ToString();
         }

         public string CreateUsnJournal()
         {
             string content = string.Empty;
             NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.CreateUsnJournal(1000 * 1024, 16 * 1024);
             if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
             {
                 content = string.Format("USN Journal Successfully created, CreateUsnJournal() returned: {0}", rtn.ToString());
             }
             else
             {
                 content = string.Format("Create->{0} returned error code: {1}", "GetUsnJournalState()", rtn.ToString());
             }
             return content;
         }


         public void ViewUsnChanges()
         {
             NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.GetUsnJournalState(ref _usnCurrentJournalState);
             if (_usnCurrentJournalState.UsnJournalID != 0)
             {
                 //Thread usnJournalThread = new Thread(ViewChangesThreadStart);
                 //usnJournalThread.Start();
                 GetFileChanges();
             }
            
      
         }

         public List<FileChange> GetFileChanges()
         {
             uint reasonMask = Win32Api.USN_REASON_DATA_OVERWRITE |
                     Win32Api.USN_REASON_DATA_EXTEND |
                     Win32Api.USN_REASON_NAMED_DATA_OVERWRITE |
                     Win32Api.USN_REASON_NAMED_DATA_TRUNCATION |
                     Win32Api.USN_REASON_FILE_CREATE |
                     Win32Api.USN_REASON_FILE_DELETE |
                     Win32Api.USN_REASON_EA_CHANGE |
                     Win32Api.USN_REASON_SECURITY_CHANGE |
                     Win32Api.USN_REASON_RENAME_OLD_NAME |
                     Win32Api.USN_REASON_RENAME_NEW_NAME |
                     Win32Api.USN_REASON_INDEXABLE_CHANGE |
                     Win32Api.USN_REASON_BASIC_INFO_CHANGE |
                     Win32Api.USN_REASON_HARD_LINK_CHANGE |
                     Win32Api.USN_REASON_COMPRESSION_CHANGE |
                     Win32Api.USN_REASON_ENCRYPTION_CHANGE |
                     Win32Api.USN_REASON_OBJECT_ID_CHANGE |
                     Win32Api.USN_REASON_REPARSE_POINT_CHANGE |
                     Win32Api.USN_REASON_STREAM_CHANGE |
                     Win32Api.USN_REASON_CLOSE;

             //Win32Api.USN_JOURNAL_DATA newUsnState;
             List<Win32Api.UsnEntry> usnEntries;
             List<FileChange> fcs = new List<FileChange>();
             NtfsUsnJournal.UsnJournalReturnCode rtnCode = _usnJournal.GetUsnJournalEntries(_usnCurrentJournalState, reasonMask, out usnEntries, out _usnCurrentJournalState);
             if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS && usnEntries.Count >0)
             {
                 //List<FileAndDirectoryEntry> ues = FileQueryEngine.GetAllFileEntrys(_volume);
                 foreach (Win32Api.UsnEntry usnEntry in usnEntries)
                 {
                     FileChange pfc = fcs.Where(o=>o.FileReferenceNumber == usnEntry.FileReferenceNumber).FirstOrDefault();
                     if (pfc == null)
                     {
                         //FileAndDirectoryEntry ue = ues.Where(o => o.FileReferenceNumber == usnEntry.FileReferenceNumber).FirstOrDefault();
                         string path = String.Empty;
                         _usnJournal.GetPathFromFileReference(usnEntry.FileReferenceNumber,out path);
                         FileChange fc = new FileChange
                         {
                             FileName = usnEntry.Name
                             ,FileReferenceNumber = usnEntry.FileReferenceNumber
                             ,ChangeType = GetChangeType(usnEntry.Reason)
                             ,FilePath = path
                             ,IsFile = usnEntry.IsFile
                             ,ChangeDate = DateTime.FromFileTime(usnEntry.ChangeDateTime)
                         };
                         fcs.Add(fc);
                     }
                     else
                     {
                         string nct = GetChangeType(usnEntry.Reason);
                         pfc.ChangeType += "," + nct;
                         if (nct == "RENAME_NEW_NAME") pfc.FileName += "," + usnEntry.Name;
                     }
                 }
             }
             return fcs;
            
         }


         private string GetChangeType(uint reason)
         {
             string content = string.Empty;
             if (0 !=(reason & Win32Api.USN_REASON_DATA_OVERWRITE))
                 content="DATA_OVERWRITE";
             if (0 != (reason & Win32Api.USN_REASON_DATA_EXTEND))
                 content="DATA_EXTEND";
             if (0 != (reason & Win32Api.USN_REASON_DATA_TRUNCATION))
                 content="DATA_TRUNCATION";
             if (0 != (reason & Win32Api.USN_REASON_NAMED_DATA_OVERWRITE))
                 content="NAMED_DATA_OVERWRITE";
             if (0 != (reason & Win32Api.USN_REASON_NAMED_DATA_EXTEND))
                 content="NAMED_DATA_EXTEND";
             if (0 != (reason & Win32Api.USN_REASON_NAMED_DATA_TRUNCATION))
                 content="NAMED_DATA_TRUNCATION";
             if (0 != (reason & Win32Api.USN_REASON_FILE_CREATE))
                 content="FILE_CREATE";
             if (0 != (reason & Win32Api.USN_REASON_FILE_DELETE))
                 content="FILE_DELETE";
             if (0 != (reason & Win32Api.USN_REASON_EA_CHANGE))
                 content="EA_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_SECURITY_CHANGE))
                 content="SECURITY_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_RENAME_OLD_NAME))
                 content="RENAME_OLD_NAME";
             if (0 != (reason & Win32Api.USN_REASON_RENAME_NEW_NAME))
                 content="RENAME_NEW_NAME";
             if (0 != (reason & Win32Api.USN_REASON_INDEXABLE_CHANGE))
                 content="INDEXABLE_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_BASIC_INFO_CHANGE))
                 content="BASIC_INFO_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_HARD_LINK_CHANGE))
                 content="HARD_LINK_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_COMPRESSION_CHANGE))
                 content="COMPRESSION_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_ENCRYPTION_CHANGE))
                 content="ENCRYPTION_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_OBJECT_ID_CHANGE))
                 content="OBJECT_ID_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_REPARSE_POINT_CHANGE))
                 content="REPARSE_POINT_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_STREAM_CHANGE))
                 content="STREAM_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_TRANSACTED_CHANGE))
                 content="TRANSACTED_CHANGE";
             if (0 != (reason & Win32Api.USN_REASON_CLOSE))
                 content="CLOSE";

             return content;

         }





    }
}

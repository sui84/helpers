using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Common.Utils.Log;
using Common.Utils.Ntfs;

namespace CommonWindowsService
{
    public partial class CommonService : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        Log log = Log4netHelper.GetLogger("TEST");
        List<UsnJournalHelper> eujhelpers = new List<UsnJournalHelper>();

        public CommonService()
        {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = Properties.Settings.Default.TimeInterval;
            string[] drivers = { "C:", "D:", "E:" };
            foreach (string driver in drivers)
            {
                UsnJournalHelper eujhelper = new UsnJournalHelper(driver);
                string str = eujhelper.QueryUsnJournal();
                eujhelpers.Add(eujhelper);
                log.InfoWithTime(str);
            }
            
        }

         void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //log.InfoWithTime("CommonWindowsService timer_Elapsed\n");
            foreach (UsnJournalHelper eujhelper in eujhelpers)
            {
                List<FileChange> fcs = eujhelper.GetFileChanges();
                foreach (FileChange fc in fcs)
                {
                    log.InfoWithTime(FormatFileChange(fc));
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            timer.Enabled = true;
            timer.Start();
            log.InfoWithTime("CommonWindowsService OnStart\n");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            timer.Stop();
            log.InfoWithTime("CommonWindowsService OnStop\n");
        }

        private string FormatFileChange(FileChange fc)
        {
            string str = string.Format("FileReferenceNumber: {0}\tFileName: {1}\tIsFile: {2}\tChangeType: {3}\tFilePath: {4}\tChangeDate: {5} {6}\n"
            , fc.FileReferenceNumber, fc.FileName, fc.IsFile, fc.ChangeType, fc.FilePath, fc.ChangeDate.ToLongDateString(), fc.ChangeDate.ToLongTimeString());
            return str;
        }
    }
}

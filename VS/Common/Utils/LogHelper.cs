using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Common.Utils
{
    public class LogHelper
    {
        public void LogInfo(string msg,string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) filePath = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            File.AppendAllText(filePath, String.Format("[{0}]{1}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"), msg), Encoding.UTF8);
        }


        public static void EventLogInfo(string source, string message)
        {
            EventLog.WriteEntry(source, message);
        }

        public static void EventLogError(string source, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            Exception _ex = ex;
            string indent = "";
            while (_ex != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(indent).AppendFormat("------------ Inner exception of type {0} --------------", _ex.GetType().Name).AppendLine();
                }
                sb.AppendFormat("{0}Type: {1}", indent, _ex.GetType()).AppendLine();
                sb.AppendFormat("{0}Message: {1}", indent, _ex.Message).AppendLine();
                StringReader sr = new StringReader(_ex.StackTrace);
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(indent).AppendLine(line);
                }
                sr.Close();
                indent += "    ";
                _ex = _ex.InnerException;
            }
            EventLog.WriteEntry(source, sb.ToString(), EventLogEntryType.Error);
        }

        public static void EventLogWarning(string source, string message)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Warning);
        }
    }
}

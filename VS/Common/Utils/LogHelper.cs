using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common.Utils
{
    public class LogHelper
    {
        public void LogInfo(string msg,string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) filePath = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            File.AppendAllText(filePath, String.Format("[{0}]{1}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"), msg), Encoding.UTF8);
        }
    }
}

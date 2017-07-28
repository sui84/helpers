using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Common.Utils.Log
{
    public static class AppLogger
    {
        private static Object staticLock = new object();

        static AppLogger()
        {
            //Logger.SetLogWriter(new LogWriterFactory().Create());
            //IConfigurationSource config = ConfigurationSourceFactory.Create();
            //ExceptionPolicyFactory factory = new ExceptionPolicyFactory(config);

            //ExceptionManager exManager = factory.CreateManager();
            //ExceptionPolicy.SetExceptionManager(factory.CreateManager());
        }

        public static void LogError(Exception ex)
        {
            lock (staticLock)
            {
                string ExceptionPolicyName = string.Empty;
                ExceptionPolicy.HandleException(ex, ExceptionPolicyName);
            }
        }

        public static void LogErrorOnly(Exception ex)
        {
            lock (staticLock)
            {
                try
                {
                    string LogOnlyPolicyName = string.Empty;
                    ExceptionPolicy.HandleException(ex, LogOnlyPolicyName);
                }
                catch(Exception excep) {
                    LogHelper logHelper = new LogHelper();
                    logHelper.LogInfo(excep.Message,string.Empty);
                }
            }
        }


        public static bool HandleException(Exception ex, string exceptionPolicyName)
        {
            lock (staticLock)
            {
                return ExceptionPolicy.HandleException(ex, exceptionPolicyName);
            }
        }

        public static void Write(object message)
        {
            lock (staticLock)
            {
                Logger.Write(message);
            }
        }

        public static void Write(object message, string category)
        {
            lock (staticLock)
            {
                Logger.Write(message, category);
            }
        }

        public static void Write(object message, TraceEventType severity)
        {
            lock (staticLock)
            {
                LogEntry entry = new LogEntry();
                entry.Message = message == null ? "" : message.ToString();
                entry.Severity = severity;
                Logger.Write(entry);
            }
        }

        public static void Debug(object message)
        {
            lock (staticLock)
            {
                string msg = string.Format("[{0:yyyy/MM/dd HH:mm:ss.fff}] {1}", DateTime.Now, message);
                try
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                }
                catch (Exception ex)
                {
                    LogErrorOnly(new ApplicationException(string.Format("Could not write debug message: {0}", msg), ex));
                }
            }
        }
    }
}

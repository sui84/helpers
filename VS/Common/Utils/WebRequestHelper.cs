using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Common.Utils.Web
{
    public class WebRequestHelper
    {
        public static string GetHostNameOrIP()
        {
            string hostname = null;
            String clientIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(clientIP))
            {
                clientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            }
            try
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry(clientIP);
                if (iPHostEntry != null)
                {
                    hostname = iPHostEntry.HostName;
                }
            }
            catch { }

            if (string.IsNullOrEmpty(hostname))
                hostname = clientIP;

            return hostname;
        }
    }
}

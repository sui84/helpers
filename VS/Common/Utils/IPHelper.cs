using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;

namespace Common.Utils
{
    public class IPHelper
    {
        public static string getIPAddress(string ipStr)
        {
            //Get IP address
            //Request.UserHostName  
            //::1
            //Request.UserHostAddress
            //::1
            //HttpContext.Current.User.Identity.Name
            if ( String.IsNullOrEmpty(ipStr) || ipStr == "127.0.0.1" || ipStr == "::1")
            {
                IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ip in arrIPAddresses)
                {
                    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork)) ipStr = ip.ToString();
                }
            }
            return ipStr;
        }

        
    }
}

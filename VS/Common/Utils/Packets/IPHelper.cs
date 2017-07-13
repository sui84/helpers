using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Common.Packets.Utils
{
    public class IPHelper
    {
        public static string GetIP4Address(string ipStr="")
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

        public static bool ValidIP(string ipStr)
        {
            Regex rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (rx.IsMatch(ipStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IPAddress IPStrToIPAddress(string ipStr)
        {
            IPAddress ipaddr = IPAddress.Parse(ipStr);
            return ipaddr;
        }

        public static IPAddress IPIntToIPAddress(long ipInt)
        {
            //使用long ulong int 会溢出，使用uint就没问题
            uint netInt = (uint)IPAddress.HostToNetworkOrder((Int32)ipInt);
            IPAddress ipaddr = new IPAddress((long)netInt);
            return ipaddr;
        }

        public static string IPLongToStr(long iplong)
        {
            // 数字转换为字符串
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(iplong.ToString());
            string strdreamduip = ipaddress.ToString();
            return strdreamduip;
        }

        //通用转换函数
        /// <summary>将IP地址格式化为整数型</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long IPStrToLong(string ip)
        {
            char[] dot = new char[] { '.' };
            string[] ipArr = ip.Split(dot);
            if (ipArr.Length == 3)ip = ip + ".0";
            ipArr = ip.Split(dot);long ip_Int = 0;
            long p1 = long.Parse(ipArr[0]) * 256 * 256 * 256;
            long p2 = long.Parse(ipArr[1]) * 256 * 256;
            long p3 = long.Parse(ipArr[2]) * 256;long p4 = long.Parse(ipArr[3]);
            ip_Int = p1 + p2 + p3 + p4;return ip_Int;
        }
        
    }
}

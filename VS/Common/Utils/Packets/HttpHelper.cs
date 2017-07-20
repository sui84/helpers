using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;

namespace Common.Packets.Utils
{
    public class HttpHelper
    {
        #region 模拟客户端socket连接
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;
            // Get host related information.
            if (NetHelper.ValidIP(server))
            {
                hostEntry = new IPHostEntry();
                hostEntry.AddressList = new IPAddress[] { IPAddress.Parse(server) };
            }
            else
            {
                hostEntry = Dns.GetHostEntry(server);
            }

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tempSocket.Connect(ipe);
                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
        #endregion
        #region 请求的主方法 request 是http请求的头部，可以用抓包工具获取,server可以使域名或者是ip地址，port http协议一般是80
        public static string SocketSendReceive(string request, string server, int port)
        {
            try
            {
                Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
                Byte[] bytesReceived = new Byte[655350];
                // 创建连接
                Socket s = ConnectSocket(server, port);
                if (s == null)
                    return ("Connection failed");
                // 发送内容.
                s.Send(bytesSent, bytesSent.Length, 0);
                // Receive the server home page content.
                int bytes = 0;
                string page = "Default HTML page on " + server + ":\r\n";
                //接受返回的内容.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    //  page = page + Encoding.UTF8.GetString(bytesReceived, 0, bytes);
                    page = page + Encoding.GetEncoding("gb2312").GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);

                return page;
            }
            catch (Exception ex)
            {
                return string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region 发送GET请求
        public static string SendGetRequest(string url, string cookie = "")
        {
            Uri uri = new Uri(url);
            string hostname = uri.Host.ToString();
            int port = uri.Port;
            string path = uri.PathAndQuery;
            string header = GetHttpHeader("GET", hostname, path, cookie);
            string getHtml = HttpHelper.SocketSendReceive(header, hostname, port);
            return getHtml;
        }
        #endregion

        public static string GetHttpHeader(string method, string hostname, string path, string cookie, string paras = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} {1} HTTP/1.1", method, path));
            sb.AppendLine(string.Format("Host: {0}", hostname));
            // 用这个参数post时候会导致400 bad request错误
            // sb.AppendLine("Connection: keep-alive");
            sb.AppendLine("Accept: */*");
            sb.AppendLine("User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36");
            sb.AppendLine("Accept-Encoding:deflate, sdch");
            sb.AppendLine("Accept-Language: zh-CN,zh;q=0.8");
            sb.AppendLine("Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
            sb.AppendLine(string.Format("Cookie: {0}", cookie));
            sb.AppendLine(string.Format("Content-Length:{0}", paras.Length));
            sb.AppendLine("Connection:close");
            sb.AppendLine("");
            sb.AppendLine(paras);
            sb.AppendLine("\r\n");  //最后一定要有回车
            string header = sb.ToString();
            return header;
        }

        #region 发送POST请求
        public static string SendPostRequest(string url, string paras, string cookie = "")
        {
            Uri uri = new Uri(url);
            string hostname = uri.Host.ToString();
            int port = uri.Port;
            string path = uri.PathAndQuery;
            string header = GetHttpHeader("POST", hostname, path, cookie, paras);
            string getHtml = HttpHelper.SocketSendReceive(header, hostname, port);
            return getHtml;
        }

        #endregion
    }
}

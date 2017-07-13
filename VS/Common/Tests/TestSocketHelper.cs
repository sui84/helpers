using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using Common.Packets.Utils;
using Common.Utils.Packets;

namespace Common.Tests
{
    public class TestSocketHelper
    {
        static void Main(string[] args)
        {
            //SocketServerHelper.Start();
            //SocketClientHelper.Start();

            IcmpHelper icmp = new IcmpHelper();
            string ping = icmp.PingHost("www.baidu.com");

            string html = HttpHelper.SendGetRequest("http://click.fanyi.baidu.com/?src=1&locate=zh&action=query&type=1&page=1");
            string html2 = HttpHelper.SendPostRequest("http://fanyi.baidu.com/v2transapi", "from=en&to=zh&query=GTTT&transtype=translang&simple_means_flag=3");

        }
    }
}

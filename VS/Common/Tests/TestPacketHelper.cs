using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using Common.Packets.Utils;

namespace Common.Tests
{
    public class TestPacketHelper
    {
        static void Main(string[] args)
        {
            PacketHelper phelper = new PacketHelper();
            phelper.GetPackageFromFile(@"d:\temp\post3.cap");
            phelper.GetPackageFromFile(@"d:\temp\post2.cap");
        }
    }
}

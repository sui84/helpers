using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Data;
using System.Reflection;


namespace Common.Utils
{
    public class PacketHelper
    {
        private static int packetIndex = 1;
        private static string captureFile ;
        private Common.Data.PacketDB pstr = new Common.Data.PacketDB();
        private static string[] httpMethods = new string[] { "GET", "POST", "HEAD", "PUT", "DELETE", "CONNECT", "OPTIONS","TRACE"};

        public void GetPackageFromFile(string capFile){
                        ICaptureDevice device;

            try
            {
                captureFile = capFile;
                // Get an offline device
                device = new CaptureFileReaderDevice( capFile );

                // Open the device
                device.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("Caught exception when opening file" + e.ToString());
                return;
            }

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival +=
                new PacketArrivalEventHandler( device_OnPacketArrival );


            // Start capture 'INFINTE' number of packets
            // This method will return when EOF reached.
            device.Capture();
            pstr.SaveChanges();

            // Close the pcap device
            device.Close();
        }
      

         private  void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            if(e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                ConnStrHelper connStrHelper = new ConnStrHelper();
                string connStr = connStrHelper.GetMSSQLClientConnStr(@"localhost\SQLEXPRESS", "Packet");
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                EthernetPacket ethernetPacket = (PacketDotNet.EthernetPacket)packet;
                
                Common.Data.Packet p = new Common.Data.Packet();
                p.HeaderData = packet.ToString(StringOutputType.VerboseColored);
                p.FilePath = captureFile;
                p.FrameNumber = packetIndex;
                p.BytesLength = packet.BytesHighPerformance.BytesLength;
                p.Timeval = e.Packet.Timeval.Date;
                if (packetIndex == 425)
                {
                    string s = System.Text.UTF8Encoding.UTF8.GetString(packet.Bytes);
                }
                int index = 1;
                var newPacket = packet;
                while (newPacket != null)
                {
                    SetPacketValue(newPacket, index, ref p);
                    if (newPacket.PayloadData != null)
                    {
                        SetPacketValue(newPacket.PayloadData, ref p);
                    }
                    newPacket = newPacket.PayloadPacket;
                    index += 1;
                }
                p.CreatedDate = DateTime.Now;
                pstr.Packets.AddObject(p);
               // pstr.SaveChanges();

                Console.WriteLine("{0} At: {1}:{2}: MAC:{3} -> MAC:{4}",
                                  packetIndex,
                                  e.Packet.Timeval.Date.ToString(),
                                  e.Packet.Timeval.Date.Millisecond,
                                  ethernetPacket.SourceHwAddress,
                                  ethernetPacket.DestinationHwAddress);
                packetIndex++;
            }
        }

         private static void SetPacketValue(byte[] payloadData, ref Common.Data.Packet p)
         {
            p.BodyData = StringHelper.ConvertBytesToUTF8String(payloadData);
            string[] bodystrs = p.BodyData.Split(new string[] { " ", " ", "\r\n", "\r\n\r\n" }, StringSplitOptions.None);
            if (bodystrs.Length > 0 && httpMethods.Contains(bodystrs[0]))
            {
                
                // http request
                p.HttpPacket = new Data.HttpPacket();
                p.HttpPacket.Direction = "REQUEST";
                p.HttpPacket.Method = bodystrs[0];
                p.HttpPacket.URL = bodystrs[1];
                p.HttpPacket.Version = bodystrs[2];
                HttpHeaderHelper httpHeader = new HttpHeaderHelper(payloadData);
                p.HttpPacket.AcceptLanguage = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Accept_Language)];
                p.HttpPacket.Connection = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Connection)];
                p.HttpPacket.Host = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Host)];
                p.HttpPacket.ContentLength = Convert.ToInt32(httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Content_Length)]);
                p.HttpPacket.CacheControl = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Cache_Control)];
                p.HttpPacket.Cookie = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Cookie)];
                //p.HttpPacket.Body = StringHelper.ConvertBytesToUTF8String(httpHeader.Data);
                p.HttpPacket.Body = bodystrs[bodystrs.Length - 1];
                
            }
            else 
            {
                bodystrs = p.BodyData.Split(new string[] { " ", "\r\n", "\r\n\r\n" }, StringSplitOptions.None);
                if (bodystrs.Length > 0 && bodystrs[0].StartsWith("HTTP"))
                {
                   //http response
                    p.HttpPacket = new Data.HttpPacket();
                    HttpHeaderHelper httpHeader = new HttpHeaderHelper(payloadData);
                    p.HttpPacket.Direction = "RESPONSE";
                    p.HttpPacket.Version = bodystrs[0] ;
                    p.HttpPacket.Status = bodystrs[1] ;
                    p.HttpPacket.Server = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Server)];
                    p.HttpPacket.Date = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Date)];
                    p.HttpPacket.ContentType = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Content_Type)];
                    p.HttpPacket.LastModified = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Last_Modified)];
                    p.HttpPacket.ContentLength = Convert.ToInt32(httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Content_Length)]);
                    p.HttpPacket.UserAgent = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.User_Agent)];
                    p.HttpPacket.ContentEncoding = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Content_Encoding)];
                    p.HttpPacket.ProxyAuthorization = httpHeader.HTTPField[Convert.ToInt32(HTTPHeaderField.Proxy_Authenticate)];
                   // p.HttpPacket.Body = StringHelper.ConvertBytesToUTF8String(httpHeader.Data);
                    p.HttpPacket.Body = bodystrs[bodystrs.Length-1];

                }
            }
         }

         private static void SetPacketValue(PacketDotNet.Packet packet, int index,ref Common.Data.Packet p)
         {
             string ptype = packet.GetType().Name;
             if (ptype == "EthernetPacket")
             {
                 EthernetPacket ethernetPacket = (PacketDotNet.EthernetPacket)packet;
                 p.EthernetPacket = new Data.EthernetPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.EthernetPacket, ethernetPacket);
             }
             else if (ptype == "TcpPacket")
             {
                 TcpPacket tcpPacket = (PacketDotNet.TcpPacket)packet;
                 p.TcpPacket = new Data.TcpPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.TcpPacket, tcpPacket);
             }
             else if (ptype == "IpPacket" || packet.GetType().BaseType.Name == "IpPacket")
             {
                 IpPacket ipPacket = (PacketDotNet.IpPacket)packet;
                 p.IPPacket = new Data.IPPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.IPPacket, ipPacket);
             }
             else if (ptype == "UdpPacket")
             {
                 UdpPacket udpPacket = (PacketDotNet.UdpPacket)packet;
                 p.UdpPacket = new Data.UdpPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.UdpPacket, udpPacket);
             }
             else if (ptype == "ARPPacket")
             {
                 p.ARPPacket = new Data.ARPPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.ARPPacket, packet);
             }
             else if (ptype.StartsWith("ICMP"))
             {
                 p.ICMPPacket = new Data.ICMPPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.ICMPPacket, packet);
             }
             else if (ptype == "Ieee80211RadioPacket")
             {
                 p.Ieee80211RadioPacket = new Data.Ieee80211RadioPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.Ieee80211RadioPacket, packet);
             }
             else if (ptype.StartsWith("IGMP"))
             {
                 p.IGMPPacket = new Data.IGMPPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.IGMPPacket, packet);
             }
             else if (ptype == "LinuxSLLType")
             {
                 p.LinuxSLLType = new Data.LinuxSLLType();
                 ReflectionHelper.CopyObjectAnyNamespace(p.LinuxSLLType, packet);
             }
             else if (ptype == "PPPoEPacket")
             {
                 p.PPPoEPacket = new Data.PPPoEPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.PPPoEPacket, packet);
             }
             else if (ptype == "PPPPacket")
             {
                 p.PPPPacket = new Data.PPPPacket();
                 ReflectionHelper.CopyObjectAnyNamespace(p.PPPPacket, packet);
             }
             

             string colIndex = index.ToString();
             if (index == 1) colIndex = string.Empty; 
             PropertyInfo prop = null;
             prop = p.GetType().GetProperty("PacketType" + colIndex);
             if (prop != null) prop.SetValue(p, ptype, null);
             prop = p.GetType().GetProperty("Bytes" + colIndex);
             if (prop != null)  prop.SetValue(p, packet.Bytes, null);
             prop = p.GetType().GetProperty("BytesOffset" + colIndex);
             if (prop != null) prop.SetValue(p, packet.BytesHighPerformance.Offset, null);
             prop = p.GetType().GetProperty("NeedsCopy" + colIndex);
             if (prop != null) prop.SetValue(p, packet.BytesHighPerformance.NeedsCopyForActualBytes, null);
             prop = p.GetType().GetProperty("Color" + colIndex);
             if (prop != null) prop.SetValue(p, packet.Color, null);
             prop = p.GetType().GetProperty("Header" + colIndex);
             if (prop != null) prop.SetValue(p, packet.Header, null);
             prop = p.GetType().GetProperty("HeaderLength" + colIndex);
             if (prop != null) prop.SetValue(p, packet.Header.Length, null);
         }

    }
}

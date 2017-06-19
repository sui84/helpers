using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace ConvertPcap
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConvertHelper cvt = new ConvertHelper();
                string path = string.Empty;
                if (args.Length == 0)
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TXT/https.txt");
                else
                    path = args[0];
                DataSet ds = cvt.OpenTXT(path, ':');
                DbHelper dbhelper = new DbHelper();
                string[][] mapCols = new string[2][];
                string[] tbNames = new string[] { "ippkg", "otherpkg" };
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count > 0)
                    {
                        if (tbNames[i] == "ippkg")
                        {
                            mapCols[0] = cvt.GetIpPackageHead();
                            mapCols[1] = new string[] { "FilePath","LineNumber","EncapsulationType", "ArrivalTime", "TimeShiftForThisPacket", "EpochTime", "TimeDeltaPrevCapFrame", "TimeDeltaPrevDisFrame", "TimeSinceReferOrFirstFrame"
                                , "FrameNumber", "FrameLength", "CaptureLength", "FrameIsMarked", "FrameIsIgnored", "ProtocolsInFrame","Ethernet","Type","SenderMACAddress","SenderIPAddress","TargetMACAddress","TargetIPAddress","MSNetworkLoadBalancing"
                                , "Version", "HeaderLength", "DifferentiatedServicesField", "DifferentiatedServicesCodepoint", "ExplicitCongestionNotification", "TotalLength", "Identification"
                                , "Flags", "Reservedbit", "NotFragment", "MoreFragments", "FragmentOffset", "TimeToLive", "Protocol", "HeaderChecksum", "HeaderChecksumStatus", "Source"
                               , "SourceHost", "Destination", "DestinationHost", "SourceGeoIP", "DestinationGeoIP"
                                , "SourcePort", "DestinationPort", "StreamIndex", "TCPSegmentLen", "SequenceNumber", "NextSequenceNumber", "AcknowledgmentNumber", "HeaderLength2"
                               , "Flags2", "Reserved", "Nonce", "CongestionWindowReduced", "ECNEcho", "Urgent", "Acknowledgment", "Push", "Reset", "Syn", "Fin", "TCPFlags"
                                , "WindowSizeValue", "Checksum", "UrgentPointer", "Options"
                                , "SEQACKAnalysis", "ACKFrame", "Timestamps", "TabularDataStream", "HypertextTransferProtocol", "BinaryData","BinaryDataStr"
                               };
                        }
                        else if (tbNames[i] == "otherpkg")
                        {
                            mapCols[0] = mapCols[1] = cvt.GetOtherPackageHead();
                        }
                         // dbhelper.ClearTable(tbNames[i], Properties.Settings.Default.ConnStr);
                        dbhelper.DeleteTable(tbNames[i], string.Format(" where FilePath='{0}'", path), Properties.Settings.Default.ConnStr);
                        dbhelper.CopyData(ds.Tables[i], tbNames[i], Properties.Settings.Default.ConnStr, mapCols);
                    }

                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message + ex.StackTrace;
            }
            
            //for(int i=0;i<ds.Tables.Count;i++)
            //{
            //    DataTable dt = ds.Tables[i];
            //    string outPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,dt.TableName+".CSV");
            //    cvt.SaveCSV(dt,outPath);
            //}

        }
    }
}


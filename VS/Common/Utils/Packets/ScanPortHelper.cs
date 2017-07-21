using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Common.Utils.Packets
{
    #region
    public class OpenPorts
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class Scanner
    {
        string m_host;
        int m_port;

        public Scanner(string host, int port)
        {
            m_host = host;
            m_port = port;
        }
        public void Scan()
        {
            //TcpClient tc = new TcpClient();
            //tc.SendTimeout = tc.ReceiveTimeout = 2000;
            IPAddress ip = IPAddress.Parse(m_host);
            Socket tc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // tc.Connect(m_host, m_port);
                tc.Connect(new IPEndPoint(ip, m_port));
                if (tc.Connected)
                {
                    Console.WriteLine("Host {0} Port {1} is Open", m_host, m_port.ToString().PadRight(6));
                    ScanPortHelper.openedPorts.Add(new OpenPorts() { Host = m_host, Port = m_port });
                }
            }
            catch
            {
                Console.WriteLine("Host {0} Port {1} is Closed", m_host, m_port.ToString().PadRight(6));
            }
            finally
            {
                tc.Close();
                tc = null;
            }
        }

    }
    #endregion

    public class ScanPortHelper
    {
        internal static List<OpenPorts> openedPorts = new List<OpenPorts>();
        static int maxThread = 100;

        public void Start(string hostFile, string portFile, string outFile)
        {
            string[] hosts = File.ReadAllLines(hostFile);
            int[] ports = StringHelper.ConvertStrArrayToIntArray(File.ReadAllLines(portFile));
            Start(hosts, ports, outFile);
        }

        public void Start(string[] hosts,int[] ports , string outFile){
            try
            {
                var t = TimerHelper.TimerStart();
                FileStream filestream = new FileStream(outFile, FileMode.Create);
                var streamwriter = new StreamWriter(filestream);
                streamwriter.AutoFlush = true;
                Console.SetOut(streamwriter);
                Console.SetError(streamwriter);

                var threads = new List<Thread>();
                for (int i = 0; i < hosts.Count(); i++)
                {
                    int index = i;
                    Thread thread = new Thread(() =>
                       Parallel.For(0, ports.Count(), (new ParallelOptions { MaxDegreeOfParallelism = maxThread }), (int j) =>
                       {
                           try
                           {
                               Scanner scanner = new Scanner(hosts[index], ports[j]);
                               scanner.Scan();
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine(ex.Message);
                           }
                       })

                     );
                    thread.Name = "Thread" + i;
                    threads.Add(thread);
                }

                Parallel.ForEach(threads, thread => thread.Start());
                foreach (var thread in threads)
                {
                    thread.Join();
                }

                Console.WriteLine();
                Console.WriteLine();


                foreach (OpenPorts op in openedPorts)
                {
                    Console.WriteLine("Host: {0}\tPort: {1} is open", op.Host, op.Port.ToString().PadLeft(6));
                }


                Console.WriteLine("Spend Time: {0}", TimerHelper.TimerEnd(t));

                streamwriter.Close();
                filestream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : {0}\r\n{1}",ex.Message ,ex.StackTrace);
            }
    }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Utils;

namespace Common.Packets.Utils
{
    public static class SocketServerHelper
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口  
        static Socket serverSocket;
        public static void Start()
        {
            //服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }  
    }

    public static class SocketClientHelper
    {
        private static byte[] result = new byte[1024];
        public static void Start()
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
            //通过clientSocket接收数据  
            int receiveLength = clientSocket.Receive(result);
            Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
            //通过 clientSocket 发送数据  
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Thread.Sleep(1000);    //等待1秒钟  
                    string sendMessage = "client send Message Hellp" + DateTime.Now;
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Console.WriteLine("向服务器发送消息：{0}" + sendMessage);
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
            Console.WriteLine("发送完毕，按回车键退出");
            Console.ReadLine();
        }

        #region 获取远程客户机的IP地址
        /// <summary>
        /// 获取远程客户机的IP地址
        /// </summary>
        /// <param name="clientSocket">客户端的socket对象</param>        
        public static string GetClientIP(Socket clientSocket)
        {
            IPEndPoint client = (IPEndPoint)clientSocket.RemoteEndPoint;
            return client.Address.ToString();
        }
        #endregion

        #region 创建一个IPEndPoint对象
        /// <summary>
        /// 创建一个IPEndPoint对象
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>        
        public static IPEndPoint CreateIPEndPoint(string ip, int port)
        {
            IPAddress ipAddress = NetHelper.IPStrToIPAddress(ip);
            return new IPEndPoint(ipAddress, port);
        }
        #endregion

        #region 创建一个TcpListener对象
        /// <summary>
        /// 创建一个自动分配IP和端口的TcpListener对象
        /// </summary>        
        public static TcpListener CreateTcpListener()
        {
            //创建一个自动分配的网络节点
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 0);

            return new TcpListener(localEndPoint);
        }
        /// <summary>
        /// 创建一个TcpListener对象
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>        
        public static TcpListener CreateTcpListener(string ip, int port)
        {
            //创建一个网络节点
            IPAddress ipAddress = NetHelper.IPStrToIPAddress(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            return new TcpListener(localEndPoint);
        }
        #endregion

        #region 创建一个基于TCP协议的Socket对象
        /// <summary>
        /// 创建一个基于TCP协议的Socket对象
        /// </summary>        
        public static Socket CreateTcpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        #endregion

        #region 创建一个基于UDP协议的Socket对象
        /// <summary>
        /// 创建一个基于UDP协议的Socket对象
        /// </summary>        
        public static Socket CreateUdpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        #endregion

        #region 获取本地终结点

        #region 获取TcpListener对象的本地终结点
        /// <summary>
        /// 获取TcpListener对象的本地终结点
        /// </summary>
        /// <param name="tcpListener">TcpListener对象</param>        
        public static IPEndPoint GetLocalPoint(TcpListener tcpListener)
        {
            return (IPEndPoint)tcpListener.LocalEndpoint;
        }

        /// <summary>
        /// 获取TcpListener对象的本地终结点的IP地址
        /// </summary>
        /// <param name="tcpListener">TcpListener对象</param>        
        public static string GetLocalPoint_IP(TcpListener tcpListener)
        {
            IPEndPoint localEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
            return localEndPoint.Address.ToString();
        }

        /// <summary>
        /// 获取TcpListener对象的本地终结点的端口号
        /// </summary>
        /// <param name="tcpListener">TcpListener对象</param>        
        public static int GetLocalPoint_Port(TcpListener tcpListener)
        {
            IPEndPoint localEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
            return localEndPoint.Port;
        }
        #endregion

        #region 获取Socket对象的本地终结点
        /// <summary>
        /// 获取Socket对象的本地终结点
        /// </summary>
        /// <param name="socket">Socket对象</param>        
        public static IPEndPoint GetLocalPoint(Socket socket)
        {
            return (IPEndPoint)socket.LocalEndPoint;
        }

        /// <summary>
        /// 获取Socket对象的本地终结点的IP地址
        /// </summary>
        /// <param name="socket">Socket对象</param>        
        public static string GetLocalPoint_IP(Socket socket)
        {
            IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
            return localEndPoint.Address.ToString();
        }

        /// <summary>
        /// 获取Socket对象的本地终结点的端口号
        /// </summary>
        /// <param name="socket">Socket对象</param>        
        public static int GetLocalPoint_Port(Socket socket)
        {
            IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
            return localEndPoint.Port;
        }
        #endregion

        #endregion

        #region 绑定终结点
        /// <summary>
        /// 绑定终结点
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="endPoint">要绑定的终结点</param>
        public static void BindEndPoint(Socket socket, IPEndPoint endPoint)
        {
            if (!socket.IsBound)
            {
                socket.Bind(endPoint);
            }
        }

        /// <summary>
        /// 绑定终结点
        /// </summary>
        /// <param name="socket">Socket对象</param>        
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口</param>
        public static void BindEndPoint(Socket socket, string ip, int port)
        {
            //创建终结点
            IPEndPoint endPoint = CreateIPEndPoint(ip, port);

            //绑定终结点
            if (!socket.IsBound)
            {
                socket.Bind(endPoint);
            }
        }
        #endregion

        #region 指定Socket对象执行监听
        /// <summary>
        /// 指定Socket对象执行监听，默认允许的最大挂起连接数为100
        /// </summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="port">监听的端口号</param>
        public static void StartListen(Socket socket, int port)
        {
            //创建本地终结点
            IPEndPoint localPoint = CreateIPEndPoint(NetHelper.LocalHostName, port);

            //绑定到本地终结点
            BindEndPoint(socket, localPoint);

            //开始监听
            socket.Listen(100);
        }

        /// <summary>
        /// 指定Socket对象执行监听
        /// </summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="maxConnection">允许的最大挂起连接数</param>
        public static void StartListen(Socket socket, int port, int maxConnection)
        {
            //创建本地终结点
            IPEndPoint localPoint = CreateIPEndPoint(NetHelper.LocalHostName, port);

            //绑定到本地终结点
            BindEndPoint(socket, localPoint);

            //开始监听
            socket.Listen(maxConnection);
        }

        /// <summary>
        /// 指定Socket对象执行监听
        /// </summary>
        /// <param name="socket">执行监听的Socket对象</param>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="maxConnection">允许的最大挂起连接数</param>
        public static void StartListen(Socket socket, string ip, int port, int maxConnection)
        {
            //绑定到本地终结点
            BindEndPoint(socket, ip, port);

            //开始监听
            socket.Listen(maxConnection);
        }
        #endregion

        #region 连接到基于TCP协议的服务器
        /// <summary>
        /// 连接到基于TCP协议的服务器,连接成功返回true，否则返回false
        /// </summary>
        /// <param name="socket">Socket对象</param>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>     
        public static bool Connect(Socket socket, string ip, int port)
        {
            try
            {
                //连接服务器
                socket.Connect(ip, port);

                //检测连接状态
                return socket.Poll(-1, SelectMode.SelectWrite);
            }
            catch (SocketException ex)
            {
                throw new Exception(ex.Message);
                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
            }
        }
        #endregion

        #region 以同步方式发送消息
        /// <summary>
        /// 以同步方式向指定的Socket对象发送消息
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="msg">发送的消息</param>
        public static void SendMsg(Socket socket, byte[] msg)
        {
            //发送消息
            socket.Send(msg, msg.Length, SocketFlags.None);
        }

        /// <summary>
        /// 使用UTF8编码格式以同步方式向指定的Socket对象发送消息
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="msg">发送的消息</param>
        public static void SendMsg(Socket socket, string msg)
        {
            //将字符串消息转换成字符数组
            byte[] buffer = StringHelper.StringToBytes(msg, Encoding.Default);

            //发送消息
            socket.Send(buffer, buffer.Length, SocketFlags.None);
        }
        #endregion

        #region 以同步方式接收消息
        /// <summary>
        /// 以同步方式接收消息
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="buffer">接收消息的缓冲区</param>
        public static void ReceiveMsg(Socket socket, byte[] buffer)
        {
            socket.Receive(buffer);
        }

        /// <summary>
        /// 以同步方式接收消息，并转换为UTF8编码格式的字符串,使用5000字节的默认缓冲区接收。
        /// </summary>
        /// <param name="socket">socket对象</param>        
        public static string ReceiveMsg(Socket socket)
        {
            //定义接收缓冲区
            byte[] buffer = new byte[5000];
            //接收数据，获取接收到的字节数
            int receiveCount = socket.Receive(buffer);

            //定义临时缓冲区
            byte[] tempBuffer = new byte[receiveCount];
            //将接收到的数据写入临时缓冲区
            Buffer.BlockCopy(buffer, 0, tempBuffer, 0, receiveCount);
            //转换成字符串，并将其返回
            return StringHelper.BytesToString(tempBuffer, Encoding.Default);
        }
        #endregion

        #region 关闭基于Tcp协议的Socket对象
        /// <summary>
        /// 关闭基于Tcp协议的Socket对象
        /// </summary>
        /// <param name="socket">要关闭的Socket对象</param>
        public static void Close(Socket socket)
        {
            try
            {
                //禁止Socket对象接收和发送数据
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
            finally
            {
                //关闭Socket对象
                socket.Close();
            }
        }
        #endregion
    }
}

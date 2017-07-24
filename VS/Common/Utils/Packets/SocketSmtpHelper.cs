using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Common.Utils.Packets
{
    public  class SocketSmtpHelper
    {
        #region Member

        #region boundary normal Property
        private string boundary_ = Guid.NewGuid().ToString();//Generate unique boundary.
        public string boundary_pro
        {
            get
            {
                return boundary_;
            }
            set
            {
                boundary_ = value;
            }
        }
        #endregion

        #region exception  Property
        private Exception exception_ = null;
        public Exception exception_pro
        {
            get
            {
                return exception_;
            }
            set
            {
                exception_ = value;
            }
        }
        #endregion

        #region encoding_smtp normal Property
        private Encoding encoding_smtp_ = Encoding.ASCII;//smtp encoding must be ascii.
        private Encoding encoding_smtp_pro
        {
            get
            {
                return encoding_smtp_;
            }
        }
        #endregion

        #region encoding_subject normal Property
        private Encoding encoding_subject_ = Encoding.UTF8;
        public Encoding encoding_subject_pro
        {
            get
            {
                return encoding_subject_;
            }
            set
            {
                encoding_subject_ = value;
            }
        }
        #endregion

        #region encoding_content normal Property
        private Encoding encoding_content_ = Encoding.GetEncoding("gb2312");
        public Encoding encoding_content_pro
        {
            get
            {
                return encoding_content_;
            }
            set
            {
                encoding_content_ = value;
            }
        }
        #endregion

        #region receiveInfo normal Property
        private string receiveInfo_ = "";
        public string receiveInfo_pro
        {
            get
            {
                return receiveInfo_;
            }
            set
            {
                receiveInfo_ = value;
            }
        }
        #endregion

        #region socket normal Property
        private Socket socket_;
        public Socket socket_pro
        {
            get
            {
                return socket_;
            }
            private set
            {
                socket_ = value;
            }
        }
        #endregion

        #endregion

        private sealed class SMTP_BACKS
        {
            public const string SERVER_READY_OK = "220";
            public const string OPERATE_COMPLETE = "250";
            public const string AUTH_LOGIN_OK = "334";
            public const string USER_NAME_LOGIN_OK = "334";
            public const string PASSWORD_LOGIN_OK = "235";
            public const string DATA_OK = "354";
        }

        private sealed class SMTP_CMD
        {
            public const string AUTH_LOGIN = "auth login\r\n";
            public const string MAIL_FROM = "mail from:";
            public const string RCPT_TO = "rcpt to:";
            public const string SEND_BODY = "data\r\n";
            public const string VERIFY = "ehlo hello\r\n";
        }

        public sealed class MailInfo
        {
            #region attachments normal Property
            private List<string> attachments_ = new List<string>();
            public List<string> attachments_pro
            {
                get
                {
                    return attachments_;
                }
                set
                {
                    attachments_ = value;
                }
            }
            #endregion

            #region content normal Property
            private string content_ = "";
            public string content_pro
            {
                get
                {
                    return content_;
                }
                set
                {
                    content_ = value;
                }
            }
            #endregion

            #region password normal Property
            private string password_ = "";
            public string password_pro
            {
                get
                {
                    return password_;
                }
                set
                {
                    password_ = value;
                }
            }
            #endregion

            #region receiverAddresses normal Property
            private List<string> receiverAddresses_ = new List<string>();
            public List<string> receiverAddresses_pro
            {
                get
                {
                    return receiverAddresses_;
                }
                set
                {
                    receiverAddresses_ = value;
                }
            }
            #endregion

            #region senderAddress normal Property
            private string senderAddress_ = "";
            public string senderAddress_pro
            {
                get
                {
                    return senderAddress_;
                }
                set
                {
                    senderAddress_ = value;
                }
            }
            #endregion

            #region subject normal Property
            private string subject_ = "";
            public string subject_pro
            {
                get
                {
                    return subject_;
                }
                set
                {
                    subject_ = value;
                }
            }
            #endregion

            #region userName normal Property
            private string userName_ = "";
            public string userName_pro
            {
                get
                {
                    return userName_;
                }
                set
                {
                    userName_ = value;
                }
            }
            #endregion
        }

        #region send Function
        public bool send(string mailHostIP, int port, MailInfo mailInfo)
        {
            try
            {
                exception_pro = null;
                //if (!isLegal(mailHostIP, port, mailInfo))
                //{
                //    return false;
                //}
                socket_pro = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_pro.Connect(mailHostIP, port);
                if (!socket_pro.Connected)
                {
                    socket_pro.Close();
                    exception_pro = new Exception(Tip.ConnectFail);
                    return false;
                }

                if (!isServerReady())
                {
                    exception_pro = new Exception(Tip.ServerReadyFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!verify())
                {
                    exception_pro = new Exception(Tip.VerifyFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                //if (!authLogin())
                //{
                //    exception_pro = new Exception(Tip.AuthLoginFail + analyReceiveError());
                //    socket_pro.Close();
                //    return false;
                //}

                //if (!userNameLogin(mailInfo.userName_pro))
                //{
                //    exception_pro = new Exception(Tip.UserNameLoginFail + analyReceiveError());
                //    socket_pro.Close();
                //    return false;
                //}

                //if (!passowrdLogin(mailInfo.password_pro))
                //{
                //    exception_pro = new Exception(Tip.PasswordLoginFail + analyReceiveError());
                //    socket_pro.Close();
                //    return false;
                //}

                if (!mailFrom(mailInfo.senderAddress_pro))
                {
                    exception_pro = new Exception(Tip.MailFromFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!rcptTo(mailInfo.receiverAddresses_pro))
                {
                    exception_pro = new Exception(Tip.RcptToFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                if (!sendMail(mailInfo))
                {
                    exception_pro = new Exception(Tip.SendMailFail + analyReceiveError());
                    socket_pro.Close();
                    return false;
                }

                socket_pro.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                exception_pro = ex;
                return false;
            }
        }

        private string analyReceiveError()
        {
            if (string.IsNullOrEmpty(receiveInfo_pro))
            {
                return Tip.ReceiveInfoEmpty;
            }
            return receiveInfo_pro;
        }

        #endregion

        private int socketSend(string sendStr)
        {
            byte[] sendBuffer = encoding_smtp_pro.GetBytes(sendStr);
            return socket_pro.Send(sendBuffer);
        }
        private string receive()
        {
            byte[] receiveData = new byte[10240];
            int receiveLen = socket_pro.Receive(receiveData);
            receiveInfo_pro = encoding_smtp_pro.GetString(receiveData, 0, receiveLen);
            Console.WriteLine(receiveInfo_pro);
            return receiveInfo_pro;
        }

        #region send ready functions

        //#region isLegal Function
        //public bool isLegal(string mailHostIP, int port, MailInfo mailInfo)
        //{
        //    if (!IpPortSp.isPort(port.ToString()))
        //    {
        //        exception_pro = new Exception(Tip.PortIllegal);
        //        return false;
        //    }
        //    if (!MailSp.isMail(mailInfo.senderAddress_pro))
        //    {
        //        exception_pro = new Exception(Tip.SenderIllegal);
        //        return false;
        //    }
        //    return true;
        //}
        //#endregion

        #region isServerReady Function
        public bool isServerReady()
        {
            string back = receive();
            return back.Substring(0, 3).Equals(SMTP_BACKS.SERVER_READY_OK);//Response code from server always is 3 byte length.
        }
        #endregion

        #region verify Function
        public bool verify()
        {
            socketSend(SMTP_CMD.VERIFY);
            string[] verifyBacks = receive().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < verifyBacks.Length; index++)
            {
                string verifyBack = verifyBacks[index];
                if (verifyBack.Length <= 3)
                {
                    return false;
                }

                if (!verifyBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region authLogin function
        private bool authLogin()
        {
            socketSend(SMTP_CMD.AUTH_LOGIN);
            string authLoginBack = receive();
            if (string.IsNullOrEmpty(authLoginBack) || authLoginBack.Length <= 3)
            {
                return false;
            }
            return authLoginBack.Substring(0, 3).Equals(SMTP_BACKS.AUTH_LOGIN_OK);
        }
        #endregion

        #region userNameLogin function
        private bool userNameLogin(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }
            string base64UserName = StringHelper.StrToBase4String(userName);
            socketSend(base64UserName + "\r\n");
            string userNameLoginBack = receive();
            if (string.IsNullOrEmpty(userNameLoginBack) || userNameLoginBack.Length <= 3)
            {
                return false;
            }
            return userNameLoginBack.Substring(0, 3).Equals(SMTP_BACKS.USER_NAME_LOGIN_OK);
        }


        #endregion

        #region passowrdLogin function
        private bool passowrdLogin(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            socketSend(StringHelper.StrToBase4String(password) + "\r\n");
            string passowrdLoginBack = receive();
            if (string.IsNullOrEmpty(passowrdLoginBack) || passowrdLoginBack.Length <= 3)
            {
                return false;
            }
            return passowrdLoginBack.Substring(0, 3).Equals(SMTP_BACKS.PASSWORD_LOGIN_OK);
        }


        #endregion

        #region mailFrom function
        private bool mailFrom(string senderAddress)
        {
            socketSend(SMTP_CMD.MAIL_FROM + "<" + senderAddress + ">\r\n");
            string mailFromBack = receive();
            if (string.IsNullOrEmpty(mailFromBack) || mailFromBack.Length <= 3)
            {
                return false;
            }
            return mailFromBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE);
        }
        #endregion

        #region rcptTo function
        private bool rcptTo(List<string> receiverAddresses)
        {
            foreach (string receiverAddress in receiverAddresses)
            {
                socketSend(SMTP_CMD.RCPT_TO + "<" + receiverAddress + ">\r\n");
                string rcptBack = receive();
                if (string.IsNullOrEmpty(rcptBack) || rcptBack.Length <= 3)
                {
                    return false;
                }

                if (!rcptBack.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #endregion

        #region send mail body functions
        private bool sendMail(MailInfo mailInfo)
        {
            socketSend(SMTP_CMD.SEND_BODY);
            string back = receive();
            if (string.IsNullOrEmpty(back) || back.Length <= 3)
            {
                return false;
            }

            if (!back.Substring(0, 3).Equals(SMTP_BACKS.DATA_OK))
            {
                return false;
            }

            sendMailHeader(mailInfo);
            sendMailContent(mailInfo.content_pro);
            //   sendMailAttachment(mailInfo.attachments_pro);
            sendMailTail();

            back = receive();
            if (string.IsNullOrEmpty(back) || back.Length <= 3)
            {
                return false;
            }

            if (!back.Substring(0, 3).Equals(SMTP_BACKS.OPERATE_COMPLETE))
            {
                return false;
            }

            return true;
        }

        //private void sendMailAttachment(List<string> attachments)
        //{
        //    foreach (string attachment in attachments)//Add header per attachment.
        //    {
        //        if (!File.Exists(attachment))
        //        {
        //            continue;
        //        }
        //        sendAttachmentHeader(attachment);
        //        sendAttachmentData(attachment);
        //    }
        //}

        //private void sendAttachmentData(string attachment)
        //{
        //    byte[] attachmentData = FileSp.readFileBytes(attachment);
        //    socketSend(Convert.ToBase64String(attachmentData));
        //    socketSend("\r\n\r\n");
        //}

        private void sendAttachmentHeader(string attachment)
        {
            string contentType = "application/octet-stream";//Easy to transmission more format file;
            string header = "--" + boundary_pro + "\r\n"
                    + "Content-Type:" + contentType + ";name=" + attachment + "\r\n"
                    + "Content-Transfer-Encoding:base64\r\n"
                    + "Content-Disposition:attachment;filename=\"" + attachment + "\"\r\n";
            socketSend(header + "\r\n");
        }

        private void sendMailTail()
        {
            socketSend("\r\n.\r\n");//. is content end flag.        
        }

        private void sendMailHeader(MailInfo mailInfo)
        {
            string to = "";
            foreach (string receiverAddress in mailInfo.receiverAddresses_pro)
            {
                to += "To:" + receiverAddress + "\r\n";
            }

            //if you want to hide receiver,may assign to="To:abc@a.com\r\n";
            //string to = "To:abc@a.com\r\n";
            socketSend(to);


            string from = "From:" + mailInfo.senderAddress_pro + "\r\n";
            socketSend(from);

            //Avoid subject messy code,adopt encoding header statement.
            string subject = "Subject:=?" + encoding_subject_pro.BodyName + "?B?"
                            + StringHelper.StrToBase4String(mailInfo.subject_pro, Encoding.UTF8) + "?=\r\n";
            socketSend(subject);
            string header = "Mime-Version:1.0\r\n"
                        + "Content-type:multipart/mixed;"
                        + "boundary=\"" + boundary_pro + "\";\r\n"//define content type and boundary value.
                        + "Content-Transfer-Encoding:7bit\r\n"
                        + "This is a multi-part message in MIME format\r\n";
            socketSend(header + "\r\n");
        }

        private void sendMailContent(string content)
        {
            sendContentHeader();
            sendContentBody(content);
        }

        private void sendContentBody(string content)
        {
            //If content is too long,need split some blocks and send.
            //Length of one line is always 80 byte.
            string newContent = StringHelper.StrToBase4String(content, encoding_content_pro);
            socketSend(newContent + "\r\n");
        }

        private void sendContentHeader()
        {
            string header = "--" + boundary_pro + "\r\n"//boundary must begin --
                    + "Content-type:text/plain;charset=" + encoding_content_pro.BodyName + "\r\n"//Avoid messy code,add charset statement.
                    + "Content-Transfer-Encoding:base64\r\n";//Avoid messy code,use encoding base64.
            socketSend(header + "\r\n");
        }

        #endregion


        private class Tip//Easy to change other language tip.
        {
            public static string AuthLoginFail = "Auth login fail!";
            public static string ConnectFail = "Connect mail host fail!";
            public static string IpIllegal = "Ip is illegal!";
            public static string MailFromFail = "Input sender address fail!";
            public static string PasswordLoginFail = "Password login fail!";
            public static string PortIllegal = "Port is illegal!";
            public static string ReceiveInfoEmpty = "Receive info is empty!";
            public static string RcptToFail = "Input receiver address fail!";
            public static string SenderIllegal = "Sender mail address is illegal!";
            public static string SendMailFail = "Send mail fail!";
            public static string ServerReadyFail = "Server ready fail!";
            public static string UserNameLoginFail = "User name login fail!";
            public static string VerifyFail = "Verify identity fail!";
        }
    }
}

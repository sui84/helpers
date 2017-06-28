/*
 * Copyright (c) 2001-2006 TIBCO Software Inc.
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: TibcoHelper.cs 23381 2006-10-19 22:25:26Z $
 * 
 */

/*
 * This file contains an SSL parameter helper class to enable
 * an SSL connection to a TIBCO Enterprise Message Service server.
 *
 */
using System;
using System.Collections;
using TIBCO.EMS;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Common.Utils
{
    public class TibcoHelper
    {
        public static XmlSchemaSet XS
        {
            get
            {
                XmlSchemaSet xs = new XmlSchemaSet();
                xs.Add("http://www.test.com/test/v1", AppDomain.CurrentDomain.BaseDirectory + @"\XmlSchema\test.xsd");
                return xs;
            }
        }

        public static void initSSLParams(String serverUrl, String[] args)
        {
            if (serverUrl != null && serverUrl.IndexOf("ssl://") >= 0)
            {
                SSLParams ssl = new SSLParams(args);
                ssl.init();
            }
        }

        public static void logInfo(string msg)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            File.AppendAllText(fileName, String.Format("{0}:{1}\r\n",DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"), msg), Encoding.UTF8);
        }

        public static void sslUsage()
        {
            System.Console.WriteLine("\nSSL options:");
            System.Console.WriteLine("");
            System.Console.WriteLine(" -ssl_trace                            - trace SSL initialization");
            System.Console.WriteLine(" -ssl_trusted[n]           <file-name> - file with trusted certificates,");
            System.Console.WriteLine("                                         this parameter may repeat if more");
            System.Console.WriteLine("                                         than one file required");
            System.Console.WriteLine(" -ssl_target_hostname    <string>    - expected name in the certificate");
            System.Console.WriteLine(" -ssl_custom                           - use custom verifier (it shows names");
            System.Console.WriteLine("                                         always approves them).");
            System.Console.WriteLine(" -ssl_identity             <file-name> - client identity file");
            System.Console.WriteLine(" -ssl_password             <string>    - password to decrypt client identity");
            System.Console.WriteLine("                                         or key file");
            System.Environment.Exit(0);
        }

        private class SSLParams
        {
            bool ssl_trace = false;
            String ssl_target_hostname = null;
            ArrayList ssl_trusted = null;
            String ssl_identity = null;
            String ssl_password = null;
            bool ssl_custom = false;

            public SSLParams(String[] args)
            {
                int trusted_pi = 0;
                String trusted_suffix = "";

                int i = 0;
                while (i < args.Length)
                {
                    if (args[i].CompareTo("-ssl_trace") == 0)
                    {
                        ssl_trace = true;
                        i += 1;
                    }
                    else
                        if (args[i].CompareTo("-ssl_target_hostname") == 0)
                        {
                            if ((i + 1) >= args.Length) sslUsage();
                            ssl_target_hostname = args[i + 1];
                            i += 2;
                        }
                        else
                            if (args[i].CompareTo("-ssl_custom") == 0)
                            {
                                ssl_custom = true;
                                i += 1;
                            }
                            else
                                if (args[i].CompareTo("-ssl_identity") == 0)
                                {
                                    if ((i + 1) >= args.Length) sslUsage();
                                    ssl_identity = args[i + 1];
                                    i += 2;
                                }
                                else
                                    if (args[i].CompareTo("-ssl_password") == 0)
                                    {
                                        if ((i + 1) >= args.Length) sslUsage();
                                        ssl_password = args[i + 1];
                                        i += 2;
                                    }
                                    else
                                        if (args[i].CompareTo("-ssl_trusted" + trusted_suffix) == 0)
                                        {
                                            if ((i + 1) >= args.Length) sslUsage();
                                            String cert = args[i + 1];
                                            if (cert == null)
                                                continue;
                                            if (ssl_trusted == null)
                                                ssl_trusted = new ArrayList();
                                            ssl_trusted.Add(cert);
                                            trusted_pi++;
                                            trusted_suffix = System.Convert.ToString(trusted_pi);
                                            i += 2;
                                        }
                                        else
                                        {
                                            i++;
                                        }
                }
            }

            public void init()
            {
                EMSSSLFileStoreInfo storeInfo = new EMSSSLFileStoreInfo();

                if (ssl_trace)
                    EMSSSL.SetClientTracer(new System.IO.StreamWriter(System.Console.OpenStandardOutput()));

                if (ssl_target_hostname != null)
                    EMSSSL.SetTargetHostName(ssl_target_hostname);

                if (ssl_custom)
                {
                    HostVerifier v = new HostVerifier();
                    EMSSSL.SetHostNameVerifier(new EMSSSLHostNameVerifier(v.verifyHost));
                }

                if (ssl_trusted != null)
                {
                    for (int i = 0; i < ssl_trusted.Count; i++)
                    {
                        String certfile = (String)ssl_trusted[i];
                        storeInfo.SetSSLTrustedCertificate(certfile);
                    }
                }

                if (ssl_identity != null)
                {
                    storeInfo.SetSSLClientIdentity(ssl_identity);
                    storeInfo.SetSSLPassword(ssl_password.ToCharArray());
                }

                EMSSSL.SetCertificateStoreType(EMSSSLStoreType.EMSSSL_STORE_TYPE_FILE, storeInfo);
            }

            class HostVerifier
            {
                public HostVerifier()
                {
                }

                public bool verifyHost(Object source, EMSSSLHostNameVerifierArgs args)
                {
                    System.Console.WriteLine("-------------------------------------------");
                    System.Console.WriteLine("HostNameVerifier: "
                                             + "certCN = [" + args.m_certificateCommonName + "]\n"
                                             + "connected Host = [" + args.m_connectedHostName + "]\n"
                                             + "expected Host = [" + args.m_targetHostName + "]");
                    System.Console.WriteLine("-------------------------------------------");


                    return true;
                }
            }
        }

        /// <summary>
        /// dest is effective only when queueName is null
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="response"></param>
        /// <param name="time"></param>
        /// <param name="dest"></param>
        /// <param name="queueName"></param>
        public static void SendMsg(string correlationID, object response, TimeSpan time, Destination dest, string queueName)
        {
            string xml = XmlHelper.ObjToXmlStr(response);
            XDocument xdoc = XDocument.Parse(xml);
            xdoc.Validate(XS, (o, err) =>
            {
                throw new Exception(err.Message);
            });
            string EmsServerUrl = "tcp://localhost:7222";
            string EmsUser = "admin";
            string EmsPassword = "";
            String[] paras = { "-server", EmsServerUrl, "-user", EmsUser, "-password", EmsPassword, "-queue", queueName, xml };
            MsgProducer msgProducer = new MsgProducer(paras, dest, correlationID);
        }

        public  static void WriteLog(string msg)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now, msg);
        }
    }
}




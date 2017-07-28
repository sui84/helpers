/* 
 * Copyright (c) 2001-2012 TIBCO Software Inc.
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: MsgProducer.cs 58960 2012-03-14 17:02:09Z $
 * 
 */

/// <summary>
///  This is a sample of a basic MsgProducer.
/// 
///  This samples publishes specified message(s) on a specified
///  destination and quits.
/// 
///  Notice that the specified destination should exist in your configuration
///  or your topics/queues configuration file should allow
///  creation of the specified topic or queue. Sample configuration supplied with
///  the TIBCO EMS allows creation of any destination.
/// 
///  If this sample is used to publish messages into
///  MsgConsumer sample, the MsgConsumer
///  sample must be started first.
/// 
///  If -topic is not specified this sample will use a topic named
///  "topic.sample".
/// 
///  Usage:  MsgProducer  [options]
///                                <message-text1>
///                                ...
///                                <message-textN>
/// 
///   where options are:
/// 
///    -server    <server-url>  Server URL.
///                             If not specified this sample assumes a
///                             serverUrl of "tcp://localhost:7222"
///    -user      <user-name>   User name. Default is null.
///    -password  <password>    User password. Default is null.
///    -topic     <topic-name>  Topic name. Default value is "topic.sample"
///    -queue     <queue-name>  Queue name. No default
/// 
/// </summary>

using System;
using System.Collections;
using TIBCO.EMS;
using System.Text;
using Common;
using Common.Utils.Files;
using Common.Utils.Log;

namespace Common.Utils.Tibco
{
    public class MsgProducer
    {
        private String serverUrl = "tcp://localhost:7222";
        public String ServerUrl
        {
            get
            {
                return serverUrl;
            }
        }
        private String name = "topic.sample";
        public String Name
        {
            get
            {
                return name;
            }
        }
        private bool useTopic = true;
        public bool UseTopic
        {
            get
            {
                return useTopic;
            }
        }

        String userName = null;
        String password = null;
        ArrayList data = new ArrayList();
        Connection connection = null;
        Session session = null;
        MessageProducer msgProducer = null;
        Destination destination = null;

        public MsgProducer(String[] args, Destination dest,string correlationID)
        {
            ParseArgs(args);
            TibcoHelper.initSSLParams(serverUrl, args);
            if (dest != null)
            {
                destination = dest;
                name = dest.ToString();
            }

            //Console.WriteLine("\n------------------------------------------------------------------------");
            //Console.WriteLine("MsgProducer SAMPLE");
            //Console.WriteLine("------------------------------------------------------------------------");
            //Console.WriteLine("Server....................... " + ((serverUrl != null) ? serverUrl : "localhost"));
            //Console.WriteLine("User......................... " + ((userName != null) ? userName : "(null)"));
            //Console.WriteLine("Destination.................. " + name);
            //Console.WriteLine("Message Text................. ");

            //for (int j = 0; j < data.Count;j++)
            //{
            //    Console.WriteLine(data[j]);
            //}
            //Console.WriteLine("------------------------------------------------------------------------\n");

            TextMessage msg;
            int i;

            if (data.Count == 0)
            {
                Console.Error.WriteLine("Error: must specify at least one message text\n");
                Usage();
            }

            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory(serverUrl);

            connection = factory.CreateConnection(userName, password);

            // create the session
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);

            // create the destination
            if (destination == null)
            {
                if (useTopic)
                    destination = session.CreateTopic(name);
                else
                    destination = session.CreateQueue(name);
            }

            // create the producer
            msgProducer = session.CreateProducer(null);
             

            // publish messages
            for (i = 0; i < data.Count; i++)
            {
                msg = session.CreateTextMessage();

                msg.Text = (String)data[i];
                if (dest == null) msg.ReplyTo = session.CreateQueue(Name);
                msg.CorrelationID = correlationID;

                // publish message
                msgProducer.Send(destination, msg);

                string maskStr1 = XmlHelper.ReplaceElement(msg.Text, "EncryptionKey");
                string maskStr2 = XmlHelper.ReplaceElement(maskStr1, "FileContents");

                LogFormatHelper.LogFunction(String.Format("Publishing message to queue {0} on Server {1}\r\n{2}", name, serverUrl, maskStr2));
            }

            // close the connection
            connection.Close(); 
        }

        private void Usage()
        {
            Console.WriteLine("\nUsage: MsgProducer [options]");
            Console.WriteLine("                       <message-text-1>");
            Console.WriteLine("                       [<message-text-2>] ...\n");
            Console.WriteLine("");
            Console.WriteLine("   where options are:");
            Console.WriteLine("");
            Console.WriteLine("   -server   <server URL>  - Server URL, default is local server");
            Console.WriteLine("   -user     <user name>   - user name, default is null");
            Console.WriteLine("   -password <password>    - password, default is null");
            Console.WriteLine("   -topic    <topic-name>  - topic name, default is \"topic.sample\"");
            Console.WriteLine("   -queue    <queue-name>  - queue name, no default");
            Console.WriteLine("   -help-ssl               - help on ssl parameters");
            Environment.Exit(0);
        }

        private void ParseArgs(String[] args)
        {
            int i = 0;

            while (i < args.Length)
            {
                if (args[i].CompareTo("-server") == 0)
                {
                    if ((i + 1) >= args.Length)
                        Usage();
                    serverUrl = args[i + 1];
                    i += 2;
                }
                else
                    if (args[i].CompareTo("-topic") == 0)
                    {
                        if ((i + 1) >= args.Length)
                            Usage();
                        name = args[i + 1];
                        i += 2;
                    }
                    else
                        if (args[i].CompareTo("-queue") == 0)
                        {
                            if ((i + 1) >= args.Length)
                                Usage();
                            name = args[i + 1];
                            i += 2;
                            useTopic = false;
                        }
                        else
                            if (args[i].CompareTo("-user") == 0)
                            {
                                if ((i + 1) >= args.Length)
                                    Usage();
                                userName = args[i + 1];
                                i += 2;
                            }
                            else
                                if (args[i].CompareTo("-password") == 0)
                                {
                                    if ((i + 1) >= args.Length)
                                        Usage();
                                    password = args[i + 1];
                                    i += 2;
                                }
                                else
                                    if (args[i].CompareTo("-help") == 0)
                                    {
                                        Usage();
                                    }
                                    else
                                        if (args[i].CompareTo("-help-ssl") == 0)
                                        {
                                            TibcoHelper.sslUsage();
                                        }
                                        else
                                            if (args[i].StartsWith("-ssl"))
                                            {
                                                i += 2;
                                            }
                                            else
                                            {
                                                data.Add(args[i]);
                                                i++;
                                            }
            }
        }

    }
}

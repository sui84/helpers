/* 
 * Copyright (c) 2001-2012 TIBCO Software Inc.
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: MsgConsumer.cs 58960 2012-03-14 17:02:09Z $
 * 
 */

/// <summary>
/// This is a sample of a basic MsgConsumer.
///
/// This sample subscribes to specified destination and receives and prints all
/// received messages. 
///
/// Notice that the specified destination should exist in your configuration
/// or your topics/queues configuration file should allow creation of the
/// specified destination.
///
/// If this sample is used to receive messages published by MsgProducer
/// sample, it must be started prior to running the MsgProducer sample.
///
/// Usage:  MsgConsumer [options]
///
///    where options are:
///
///    -server    <server-url>  Server URL.
///                             If not specified this sample assumes a
///                             serverUrl of "tcp://localhost:7222"
///    -user      <user-name>   User name. Default is null.
///    -password  <password>    User password. Default is null.
///    -topic     <topic-name>  Topic name. Default value is "topic.sample"
///    -queue     <queue-name>  Queue name. No default
///    -ackmode   <mode>        Message acknowledge mode. Default is AUTO.
///                             Other values: DUPS_OK, CLIENT, EXPLICIT_CLIENT,
///                             EXPLICIT_CLIENT_DUPS_OK and NO.
///
/// </summary>

using System;
using TIBCO.EMS;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Xml.Schema;
using Common.Utils.Log;

namespace Common.Utils.Tibco
{
    public class MsgConsumer : IExceptionListener
    {
        private String serverUrl = "tcp://localhost:7222";
        public String ServerUrl { 
            get{
                return serverUrl;
            }
        }
        private String name = "topic.sample";
        public String Name {
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
        int ackMode = Session.AUTO_ACKNOWLEDGE;
        Connection connection = null;
        Session session = null;
        MessageConsumer msgConsumer = null;
        Destination destination = null;

        public MsgConsumer()
        {
        }

        public MsgConsumer(String[] args)
        { 
            ParseArgs(args);
            LogFormatHelper.LogFunction(String.Format("Init MsgConsumer in queue {0} on Server {1}", name, serverUrl));
            TibcoHelper.initSSLParams(serverUrl, args);

            Console.WriteLine("\n------------------------------------------------------------------------");
            Console.WriteLine("MsgConsumer SAMPLE");
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Server....................... " + ((serverUrl != null) ? serverUrl : "localhost"));
            Console.WriteLine("User......................... " + ((userName != null) ? userName : "(null)"));
            Console.WriteLine("Destination.................. " + name);
            Console.WriteLine("------------------------------------------------------------------------\n");

            Run();
        }


        private void Usage()
        {
            Console.WriteLine("\nUsage: MsgConsumer [options]");
            Console.WriteLine("");
            Console.WriteLine("   where options are:");
            Console.WriteLine("");
            Console.WriteLine("   -server   <server URL> - Server URL, default is local server");
            Console.WriteLine("   -user     <user name>  - user name, default is null");
            Console.WriteLine("   -password <password>   - password, default is null");
            Console.WriteLine("   -topic    <topic-name> - topic name, default is \"topic.sample\"");
            Console.WriteLine("   -queue    <queue-name> - queue name, no default");
            Console.WriteLine("   -ackmode  <ack-mode>   - acknowledge mode, default is AUTO");
            Console.WriteLine("                            other modes: CLIENT, DUPS_OK, NO,");
            Console.WriteLine("                            EXPLICIT_CLIENT and EXPLICIT_CLIENT_DUPS_OK");
            Console.WriteLine("   -help-ssl              - help on ssl parameters");
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
                                    if (args[i].CompareTo("-ackmode") == 0)
                                    {
                                        if ((i + 1) >= args.Length)
                                            Usage();
                                        if (args[i + 1].CompareTo("AUTO") == 0)
                                            ackMode = Session.AUTO_ACKNOWLEDGE;
                                        else if (args[i + 1].CompareTo("CLIENT") == 0)
                                            ackMode = Session.CLIENT_ACKNOWLEDGE;
                                        else if (args[i + 1].CompareTo("DUPS_OK") == 0)
                                            ackMode = Session.DUPS_OK_ACKNOWLEDGE;
                                        else if (args[i + 1].CompareTo("EXPLICIT_CLIENT") == 0)
                                            ackMode = Session.EXPLICIT_CLIENT_ACKNOWLEDGE;
                                        else if (args[i + 1].CompareTo("EXPLICIT_CLIENT_DUPS_OK") == 0)
                                            ackMode = Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE;
                                        else if (args[i + 1].CompareTo("NO") == 0)
                                            ackMode = Session.NO_ACKNOWLEDGE;
                                        else
                                        {
                                            Console.Error.WriteLine("Unrecognized -ackmode: " + args[i + 1]);
                                            Usage();
                                        }
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
                                                    Console.Error.WriteLine("Unrecognized parameter: " + args[i]);
                                                    Usage();
                                                }
            }
        }

        public void OnException(EMSException e)
        {
            LogFormatHelper.LogServiceError(e, "Exception in MsgConsumer.OnException");
        }

        private void Run()
        {

                Message msg = null;

                Console.WriteLine("Subscribing to destination: " + name + "\n");

                ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory(serverUrl);

                // create the connection
                connection = factory.CreateConnection(userName, password);

                
                // create the session
                session = connection.CreateSession(false, ackMode);

                // set the exception listener
                connection.ExceptionListener = this;

                // create the destination
                if (useTopic)
                    destination = session.CreateTopic(name);
                else
                    destination = session.CreateQueue(name);

                

                // create the consumer
                msgConsumer = session.CreateConsumer(destination);
                

                // start the connection
                connection.Start();
                LogFormatHelper.LogFunction(String.Format("Open connection in queue {0} on Server {1}", name, serverUrl));

                // read messages
                while (true)
                {
                    // receive the message
                    msg = msgConsumer.Receive();
                    if (msg == null)
                        break;

                    if (ackMode == Session.CLIENT_ACKNOWLEDGE ||
                        ackMode == Session.EXPLICIT_CLIENT_ACKNOWLEDGE ||
                        ackMode == Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE)
                        msg.Acknowledge();

                    HandleMsg(msg);


                }

                // close the connection
                connection.Close();
                LogFormatHelper.LogFunction(String.Format("Close connection in queue {0} on Server {1}", name, serverUrl));
        }

        public virtual void HandleMsg(Message msg)
       {

       }

    }
}

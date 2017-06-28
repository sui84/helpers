using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using LumenWorks.Framework.IO.Csv;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Utils{

    public class MailHelper
    {

        private SmtpClient client;
        private const string TestingEmail = "";
        private const string TestingEmailPwd = "";

        public MailHelper(string smtpServer)
        {

            client = new SmtpClient(smtpServer);
            if (!string.IsNullOrEmpty(TestingEmail) && !string.IsNullOrEmpty(TestingEmailPwd))
            {  
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(TestingEmail, TestingEmailPwd);
            }                  
            else
            {
                client.UseDefaultCredentials = true;
            }
  
        }

        public MailHelper(string smtpServer, string userName, string password)
        {
            client = new SmtpClient(smtpServer);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userName, password);
        }

        public enum MAIL_PRIORITY {
            High, Normal, Low
        }

        public static string FormatReceiveEmail(string email)
        {
            if (!string.IsNullOrEmpty(TestingEmail))
            {
                return TestingEmail;
            }
            return email;
        }

        public bool SendMail(string sender, string recipients, string subject, MAIL_PRIORITY priority, string body, params string[] attachmentsPath)
        {
            MailMessage mail = new MailMessage();
           // EmailLog log = new EmailLog { ActionDate = DateTime.Now, MachineName = Environment.MachineName, ActionBy = System.AppDomain.CurrentDomain.FriendlyName, Sender = sender, Recipient = recipients , Subject = subject , Body = body };
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(sender))
                {
                    string[] recipientAddresses = recipients.Split(new char[] {';'});
                    if (recipientAddresses.Count() > 0)
                    {
                        mail.From = new MailAddress(sender);
                        foreach (string recipientAddress in recipientAddresses)
                            mail.To.Add(new MailAddress(recipientAddress));
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.BodyEncoding = System.Text.Encoding.UTF8;
                        mail.IsBodyHtml = true;
                        foreach (string path in attachmentsPath)
                        {
                            Attachment attachFile = new Attachment(path);
                            mail.Attachments.Add(attachFile);
                        }
                        switch (priority)
                        {
                            case MAIL_PRIORITY.High:
                                mail.Priority = MailPriority.High;
                                break;
                            case MAIL_PRIORITY.Normal:
                                mail.Priority = MailPriority.Normal;
                                break;
                            case MAIL_PRIORITY.Low:
                                mail.Priority = MailPriority.Low;
                                break;
                        }
                        client.Send(mail);
                        if (attachmentsPath.Count() > 0) {
                          string methodName = LogFormatHelper.GetMethodDetail(MethodBase.GetCurrentMethod());
                          LogFormatHelper.LogRequestParams(new Dictionary<string,string>(),methodName, sender, recipients, subject, priority, body);
                        }
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), string.Empty);

            }
            finally
            {
                mail.Dispose();
            }
            return result;
        }

    }
}

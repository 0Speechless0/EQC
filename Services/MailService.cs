using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace EQC.Services
{
    public class MailService
    {
        private readonly string MilAdress = ConfigurationSettings.AppSettings["MailAdress"];

        private readonly string MailPd = ConfigurationSettings.AppSettings["MailPd"];

        private readonly string SMTP = ConfigurationSettings.AppSettings["SMTP"];

        private readonly string MailPort = ConfigurationSettings.AppSettings["MailPort"];

        /// <summary>
        /// Mail寄送
        /// </summary>
        /// <param name="mailTitle"></param>
        /// <param name="mailHtmlContent"></param>
        /// <param name="recipientMail"></param>
        /// <param name="recipientName"></param>
        public void SendMail(string mailTitle, string mailHtmlContent, string recipientMail, string recipientName = "")
        {
            MailMessage MyMail = new MailMessage();
            MyMail.From = new MailAddress(MilAdress);
            MyMail.To.Add(new MailAddress(recipientMail));
            MyMail.Subject = mailTitle;
            MyMail.Body = mailHtmlContent;
            MyMail.IsBodyHtml = true;

            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            SmtpClient MySMTP = new SmtpClient();
            MySMTP.Host = SMTP;
            MySMTP.Port = int.Parse(MailPort);
            MySMTP.EnableSsl = true;
            MySMTP.Credentials = new System.Net.NetworkCredential(MilAdress, MailPd);
            try
            {
                MySMTP.Send(MyMail);
                MyMail.Dispose(); //釋放資源
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace QuanLyBanHang.common
{
    public class SendEmailCus
    {
        private static string password = ConfigurationManager.AppSettings["PassWordEmail"];
        private static string email = ConfigurationManager.AppSettings["Email"];
        public static bool SendEmail(string name,string subject , string content,string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage mailMessage = new MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential()
                    {
                        UserName = email,
                        Password = password
                    };
                  

                  
                };
                MailAddress fromAddress = new MailAddress(email, name);
                mailMessage.From = fromAddress;
                mailMessage.To.Add(toMail);
                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                rs = true;
                smtp.Send(mailMessage);

            }
            catch (Exception ex)
            {
                rs = false;
                Console.WriteLine(ex.ToString());

            }
            return rs;
        }

    }
}
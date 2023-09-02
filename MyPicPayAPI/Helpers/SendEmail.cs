using System.Net;
using System.Net.Mail;

namespace SimplePicPay.Helpers
{
    public class SendEmail : ISendEmail
    {
        public async Task<bool> SendMail(string email, string subject, string message)
        {
            try
            {
                string mail = "<your-email>"; // Your valid email, preference for outlook email 😉
                string pw = "<your-password"; // Password email 🤐

                string host = "smtp-mail.outlook.com";
                int port = 587;

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(mail, "PicPay Simplificado")
                };

                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(mail, pw);
                    smtp.EnableSsl = true;

                    smtp.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}

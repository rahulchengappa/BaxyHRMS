using System.Net;
using System.Net.Mail;

namespace HRMS.Infrastructure.Services
{
    public static class EmailHelper
    {
        public static void SendResetEmail(string to, string resetUrl)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    "chengappa666@gmail.com",
                    "qdyn kwcf qltb mifs"  // GOOGLE APP PASSWORD, NOT login password
                ),
                EnableSsl = true
            };

            string subject = "HRMS Password Reset";
            string body = $"Click the link to reset your password:<br><br><a href='{resetUrl}'>Reset Password</a>";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("chengappa666@gmail.com");
            msg.To.Add(to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            client.Send(msg);
        }
    }
}

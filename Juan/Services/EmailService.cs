using System.Net.Mail;
using System.Net;
using Juan.Interfaces;

namespace Juan.Services;

public class EmailService : IEmailService
{
    public void SendEmail(string email, string subject, string body)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("mehemmed05.aliyev@gmail.com", "AllUp");
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;


        mailMessage.Body = body;

        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Port = 587;
        smtpClient.Host = "smtp.gmail.com";
        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential("mehemmed05.aliyev@gmail.com", "pmkt bkfz ntog qxrh");

        smtpClient.Send(mailMessage);
    }
}
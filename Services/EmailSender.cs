
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


// Class providing email server information to send emails
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        using var client = new SmtpClient(smtpServer, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 10000 // 10 seconds
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, "FormRequest App"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}

using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using tl121pet.Entities.Infrastructure;
using tl121pet.Services.Interfaces;
using MailKit.Security;

namespace tl121pet.Services.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        public MailService(IOptions<MailSettings> mailSettings)
        { 
            _settings = mailSettings.Value;
        }
        public async void SendMailAsync(MailRequest mail)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.Mail));
            email.To.Add(MailboxAddress.Parse(mail.ToEmail));
            email.Subject = mail.Subject;
            email.Body = new TextPart(TextFormat.Text) { Text = mail.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _settings.Host,
                _settings.Port,
                true);
            smtp.Authenticate(_settings.Mail, _settings.Password);
            await smtp.SendAsync(email);
        }
    }
}

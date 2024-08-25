using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class TlMailService(IOptions<MailSettings> mailSettings) : ITlMailService
    {
        private readonly MailSettings _infrastructureSettings = mailSettings.Value;

        public async Task SendInfrastructureMailAsync(MailRequest mail)
        {
            MimeMessage email = BuildMailMessage(mail, _infrastructureSettings.Mail);
            using var smtp = ConfigureMailServer(_infrastructureSettings);
            await smtp.SendAsync(email);
        }

        public async Task SendMailAsync(MailRequest mail, MailSettings mailSettings)
        {
            MimeMessage email = BuildMailMessage(mail, mailSettings.Mail);
            using var smtp = ConfigureMailServer(mailSettings);
            await smtp.SendAsync(email);
        }

        private MimeMessage BuildMailMessage(MailRequest mail, string senderEmail)
        {
            MimeMessage email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(mail.ToEmail));
            email.Subject = mail.Subject;
            email.Body = new TextPart(TextFormat.Text) { Text = mail.Body };

            return email;
        }

        private SmtpClient ConfigureMailServer(MailSettings settings)
        {
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.Connect(
                settings.Host,
                settings.Port,
                true);
            }
            catch
            {
                throw new LogicException("Unable to connect to mail server: check you mail settings");
            }

            try
            {
                smtp.Authenticate(settings.Mail, settings.Password);
            }
            catch
            {
                throw new LogicException("Mail Authentication failed: check you mail settings");
            }
            
            return smtp;
        }
    }
}

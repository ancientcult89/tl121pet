using tl121pet.Entities.Infrastructure;

namespace tl121pet.Services.Interfaces
{
    public interface ITlMailService
    {
        public Task SendInfrastructureMailAsync(MailRequest mail);
        public Task SendMailAsync(MailRequest mail, MailSettings mailSettings);
    }
}

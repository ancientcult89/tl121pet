using tl121pet.Entities.Infrastructure;

namespace tl121pet.Services.Interfaces
{
    public interface IMailService
    {
        public Task SendMailAsync(MailRequest mail);
    }
}

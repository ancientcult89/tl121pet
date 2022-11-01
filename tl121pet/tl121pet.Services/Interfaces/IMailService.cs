using tl121pet.Entities.Infrastructure;

namespace tl121pet.Services.Interfaces
{
    public interface IMailService
    {
        public void SendMailAsync(MailRequest mail);
    }
}

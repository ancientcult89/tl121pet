using tl121pet.Entities.Infrastructure;

namespace tl121pet.Services.Interfaces
{
    public interface ITlMailService
    {
        public Task SendMailAsync(MailRequest mail);
    }
}

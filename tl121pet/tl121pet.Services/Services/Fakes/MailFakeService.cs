using tl121pet.Entities.Infrastructure;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services.Fakes
{
    public class MailFakeService : IMailService
    {
        public void SendMailAsync(MailRequest mail)
        {
            return;
        }
    }
}

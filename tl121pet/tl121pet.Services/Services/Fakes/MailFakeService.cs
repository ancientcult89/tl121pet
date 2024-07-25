using tl121pet.Entities.Infrastructure;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services.Fakes
{
    public class MailFakeService : ITlMailService
    {
        public async Task SendInfrastructureMailAsync(MailRequest mail)
        {
            Task emptyTask = Task.CompletedTask;
            await emptyTask;
        }

        public async Task SendMailAsync(MailRequest mail, MailSettings mailSettings)
        {
            Task emptyTask = Task.CompletedTask;
            await emptyTask;
        }
    }
}

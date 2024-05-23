using tl121pet.Entities.Infrastructure;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services.Fakes
{
    public class MailFakeService : ITlMailService
    {
        public async Task SendMailAsync(MailRequest mail)
        {
            Task emptyTask = Task.CompletedTask;
            await emptyTask;
        }
    }
}

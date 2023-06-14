
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IMeetingService
    {
        public Task<List<Meeting>> GetMeetingsAsync(long? personId);
    }
}

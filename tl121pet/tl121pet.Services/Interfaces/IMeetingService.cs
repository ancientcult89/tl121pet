
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IMeetingService
    {
        public List<Meeting> GetMeetings(long? personId);
    }
}

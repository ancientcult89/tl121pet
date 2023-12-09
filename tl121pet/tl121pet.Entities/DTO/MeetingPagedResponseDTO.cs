using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.DTO
{
    public class MeetingPagedResponseDTO
    {
        public List<Meeting> Meetings = new List<Meeting>();
        public PageInfo PageInfo { get; set; }
    }
}

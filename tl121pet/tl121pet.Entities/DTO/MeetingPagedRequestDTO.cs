using tl121pet.Entities.Infrastructure;

namespace tl121pet.Entities.DTO
{
    public class MeetingPagedRequestDTO : PageInfoRequest
    {
        public long? PersonId { get; set; }
    }
}

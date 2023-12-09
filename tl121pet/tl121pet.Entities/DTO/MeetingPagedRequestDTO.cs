using tl121pet.Entities.Infrastructure;

namespace tl121pet.Entities.DTO
{
    public class MeetingPagedRequestDTO
    {
        public PageInfo PageInfo { get; set; } = new PageInfo();
        public long? PersonId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

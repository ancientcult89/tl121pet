using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class MeetingExtension
    {
        public static MeetingDTO ToDto(this Meeting meeting)
        {
            return new MeetingDTO()
            {
                MeetingId = meeting.MeetingId,
                MeetingDate = meeting.MeetingDate,
                MeetingPlanDate = meeting.MeetingPlanDate,
                PersonId = meeting.PersonId,
                FollowUpIsSended = meeting.FollowUpIsSended
            };
        }
    }
}

using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class MeetingDtoExtension
    {
        public static Meeting ToEntity(this MeetingDTO meetingDTO)
        {
            return new Meeting()
            {
                MeetingId = meetingDTO.MeetingId,
                MeetingDate = meetingDTO.MeetingDate,
                MeetingPlanDate = meetingDTO.MeetingPlanDate,
                FollowUpIsSended = meetingDTO.FollowUpIsSended,
                PersonId = meetingDTO.PersonId
            };
        }
    }
}

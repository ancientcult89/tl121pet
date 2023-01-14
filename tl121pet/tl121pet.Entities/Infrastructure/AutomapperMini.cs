using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Infrastructure
{
    public static class AutomapperMini
    {
        public static MeetingDTO MeetingEntityToDto(Meeting meeting)
        {
            return new MeetingDTO()
            {
                MeetingId = meeting.MeetingId,
                MeetingDate = meeting.MeetingDate,
                MeetingPlanDate = meeting.MeetingPlanDate,
                MeetingTypeId = meeting.MeetingTypeId,
                PersonId = meeting.PersonId,
                FollowUpIsSended = meeting.FollowUpIsSended
            } ?? new MeetingDTO();
        }

        public static Meeting MeetingDtoToEntity(MeetingDTO meetingDTO)
        {
            return new Meeting()
            {
                MeetingId = meetingDTO.MeetingId,
                MeetingDate = meetingDTO.MeetingDate,
                MeetingPlanDate = meetingDTO.MeetingPlanDate,
                MeetingTypeId = meetingDTO.MeetingTypeId,
                FollowUpIsSended= meetingDTO.FollowUpIsSended,
                PersonId = meetingDTO.PersonId
            };
        }
    }
}

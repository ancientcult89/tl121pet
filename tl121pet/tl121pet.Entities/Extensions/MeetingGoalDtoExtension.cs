using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class MeetingGoalDtoExtension
    {
        public static MeetingGoal ToEntity(this MeetingGoalDTO meetingGoalDTO)
        {
            MeetingGoal meetingGoal = new MeetingGoal()
            {
                IsCompleted = default,
                MeetingGoalDescription = meetingGoalDTO.MeetingGoalDescription,
                MeetingId = meetingGoalDTO.MeetingId,
            };
            if (meetingGoalDTO.MeetingGoalId != null)
                meetingGoal.MeetingGoalId = (Guid)meetingGoalDTO.MeetingGoalId;

            return meetingGoal;
        }
    }
}

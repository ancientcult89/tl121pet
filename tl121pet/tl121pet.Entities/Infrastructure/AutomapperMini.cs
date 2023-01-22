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

        public static UserDTO UserEntityToDto(User user)
        {
            return new UserDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }

        public static User UserDtoToEntity(UserDTO userDTO, byte[] passwordHash, byte[] passwordSalt)
        {
            return new User() {
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = userDTO.RoleId
            };
        }

        public static TaskDTO MeetingGoalEntityToDto(MeetingGoal meetingGoal)
        {
            return new TaskDTO()
            {
                MeetingGoalId = meetingGoal.MeetingGoalId,
                CompleteDescription = meetingGoal.CompleteDescription,
                IsCompleted = meetingGoal.IsCompleted,
                MeetingGoalDescription = meetingGoal.MeetingGoalDescription
            };
        }
    }
}

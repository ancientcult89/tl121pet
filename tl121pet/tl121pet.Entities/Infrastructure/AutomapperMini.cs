using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Infrastructure
{
    public class AutomapperMini : IAutomapperMini
    {
        public MeetingDTO MeetingEntityToDto(Meeting meeting)
        {
            return new MeetingDTO()
            {
                MeetingId = meeting.MeetingId,
                MeetingDate = meeting.MeetingDate,
                MeetingPlanDate = meeting.MeetingPlanDate,
                PersonId = meeting.PersonId,
                FollowUpIsSended = meeting.FollowUpIsSended
            } ?? new MeetingDTO();
        }

        public Meeting MeetingDtoToEntity(MeetingDTO meetingDTO)
        {
            return new Meeting()
            {
                MeetingId = meetingDTO.MeetingId,
                MeetingDate = meetingDTO.MeetingDate,
                MeetingPlanDate = meetingDTO.MeetingPlanDate,
                FollowUpIsSended= meetingDTO.FollowUpIsSended,
                PersonId = meetingDTO.PersonId
            };
        }

        public UserDTO UserEntityToDto(User user)
        {
            return new UserDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }

        public User UserDtoToEntity(UserDTO userDTO, byte[] passwordHash, byte[] passwordSalt)
        {
            return new User() {
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = userDTO.RoleId
            };
        }
    }
}

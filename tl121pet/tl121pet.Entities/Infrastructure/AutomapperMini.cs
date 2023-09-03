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

        public MeetingNote MeetingNoteDtoToEntity(MeetingNoteDTO meetingNoteDTO)
        {
            MeetingNote note = new MeetingNote()
            {
                MeetingId = meetingNoteDTO.MeetingId,
                FeedbackRequired = meetingNoteDTO.FeedbackRequired,
                MeetingNoteContent = meetingNoteDTO.MeetingNoteContent,
            };
            if(meetingNoteDTO.MeetingNoteId != null)
                note.MeetingNoteId = (Guid)meetingNoteDTO.MeetingNoteId;

            return note;
        }

        public MeetingNoteDTO MeetingNoteEntityToDto(MeetingNote meetingNote)
        {
            return new MeetingNoteDTO() { 
                MeetingNoteId = meetingNote.MeetingNoteId,
                MeetingId = meetingNote.MeetingId,
                MeetingNoteContent = meetingNote.MeetingNoteContent,
                FeedbackRequired = meetingNote.FeedbackRequired,
            };
        }
    }
}

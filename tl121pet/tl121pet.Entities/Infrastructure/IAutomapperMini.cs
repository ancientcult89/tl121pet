using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Infrastructure
{
    public interface IAutomapperMini
    {
        Meeting MeetingDtoToEntity(MeetingDTO meetingDTO);
        MeetingDTO MeetingEntityToDto(Meeting meeting);

        MeetingNote MeetingNoteDtoToEntity(MeetingNoteDTO meetingNoteDTO);
        MeetingNoteDTO MeetingNoteEntityToDto(MeetingNote meetingNote);

        MeetingGoal MeetingGoalDtoToEntity(MeetingGoalDTO meetingGoalDTO);
        MeetingGoalDTO MeetingGoalEntityToDto(MeetingGoal meetingGoal);

        User UserDtoToEntity(UserDTO userDTO, byte[] passwordHash, byte[] passwordSalt);
        UserDTO UserEntityToDto(User user);
    }
}
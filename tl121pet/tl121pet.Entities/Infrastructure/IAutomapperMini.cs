using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Infrastructure
{
    public interface IAutomapperMini
    {
        Meeting MeetingDtoToEntity(MeetingDTO meetingDTO);
        MeetingDTO MeetingEntityToDto(Meeting meeting);
        User UserDtoToEntity(UserDTO userDTO, byte[] passwordHash, byte[] passwordSalt);
        UserDTO UserEntityToDto(User user);
    }
}
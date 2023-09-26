using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class UserDtoExtension
    {
        public static User ToEntity(this UserDTO userDTO, byte[] passwordHash, byte[] passwordSalt)
        {
            return new User()
            {
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = userDTO.RoleId
            };
        }
    }
}

using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class UserExtension
    {
        public static UserDTO ToDto(this User user)
        {
            return new UserDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            };
        }
    }
}

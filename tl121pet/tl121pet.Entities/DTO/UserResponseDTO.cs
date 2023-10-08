using tl121pet.Entities.Models;

namespace tl121pet.Entities.DTO
{
    public class UserResponseDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}

using tl121pet.Entities.Models;

namespace tl121pet.Entities.DTO
{
    public class LoginResponseDTO
    {
        public User User { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}

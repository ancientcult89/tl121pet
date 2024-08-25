using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class UserRegisterRequestDTO
    {
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class UserLoginRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

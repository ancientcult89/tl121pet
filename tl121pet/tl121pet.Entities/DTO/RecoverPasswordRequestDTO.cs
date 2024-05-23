using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class RecoverPasswordRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

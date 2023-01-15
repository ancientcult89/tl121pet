using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class ChangeUserPasswordRequestDTO
    {
        public long UserId { get; set; }

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required, Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

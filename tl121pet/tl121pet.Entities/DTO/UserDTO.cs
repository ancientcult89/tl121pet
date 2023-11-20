using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class UserDTO
    {
        public long Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [Range(1, long.MaxValue)]
        public int? RoleId { get; set; }
    }
}

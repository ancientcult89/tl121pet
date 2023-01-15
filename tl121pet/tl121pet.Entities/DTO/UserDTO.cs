using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{

    //TODO: необходимо разделить ДТО на 2: редактирование и создание
    public class UserDTO
    {
        public long Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "Required")]
        public int RoleId { get; set; }
    }
}

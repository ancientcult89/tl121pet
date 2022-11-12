using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class UserEditRequest
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}

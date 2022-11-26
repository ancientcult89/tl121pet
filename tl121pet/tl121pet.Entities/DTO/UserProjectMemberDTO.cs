
namespace tl121pet.Entities.DTO
{
    public class UserProjectMemberDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Projects { get; set; } = string.Empty;
    }
}

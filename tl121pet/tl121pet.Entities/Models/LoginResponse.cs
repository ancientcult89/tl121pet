namespace tl121pet.Entities.Models
{
    public class LoginResponse
    {
        public User User { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}

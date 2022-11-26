using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IAuthService
    {
        public string GetMyRole();
        public long? GetMyUserId();
        public void Register(UserRegisterRequest request);
        public User? Login(UserLoginRequest request);
        public string CreateToken(User user);
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        public string Role { get; set; }
    }
}

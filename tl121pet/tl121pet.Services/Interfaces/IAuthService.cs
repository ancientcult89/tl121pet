using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IAuthService
    {
        public string GetMyRole();
        public long? GetMyUserId();
        public Task Register(UserRegisterRequestDTO request);
        public Task<User?> LoginAsync(UserLoginRequestDTO request);
        public Task<string> CreateTokenAsync(User user);
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        public string Role { get; set; }

        public Task<User?> GetUserByEmailAsync(string email);
        public Task<string> GetRoleNameByIdAsync(int id);
        public Task<User?> GetUserByIdAsync(long id);
        public Task<List<ProjectTeam>> GetUserProjectsAsync(long userId);
        public Task UpdateRoleAsync(Role role);
        public Task CreateRoleAsync(Role role);
        public Task DeleteRoleAsync(int roleId);
        public Task CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(long userId);
        public Task<List<User>> GetUserListAsync();
        public Task<List<Role>> GetRoleListAsync();
    }
}

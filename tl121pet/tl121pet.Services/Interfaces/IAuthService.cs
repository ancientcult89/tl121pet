using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IAuthService
    {
        public long? GetMyUserId();
        public Task RegisterAsync(UserRegisterRequestDTO request);
        public Task ChangePasswordAsync(ChangeUserPasswordRequestDTO changeUserPasswordRequest);
        public Task ChangeLocaleByUserIdAsync(long userId, Locale locale);
        public Task<LoginResponseDTO> LoginAsync(UserLoginRequestDTO request);
        public Task<string> CreateTokenAsync(User user);
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        public string Role { get; set; }

        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserByNameAsync(string userName);
        public Task<string> GetRoleNameByIdAsync(int? id);
        public Task<User?> GetUserByIdAsync(long id);
        public Task<List<ProjectTeam>> GetUserProjectsAsync(long userId);
        public Task<User> CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task<UserDTO> UpdateUserAsync(UserDTO user);
        public Task DeleteUserAsync(long userId);
        public Task<List<User>> GetUserListAsync();

    }
}

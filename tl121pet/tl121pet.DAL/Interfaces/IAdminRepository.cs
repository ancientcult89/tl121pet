using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IAdminRepository
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<string> GetRoleNameByIdAsync(int id);
        public Task<User?> GetUserByIdAsync(long id);
        
        //TODO: пересекается с методом в IProjectTeamRepository, нужно избавиться от дубляжа в названии, может как-то переименовать или там или тут
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

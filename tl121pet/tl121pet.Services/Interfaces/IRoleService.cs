using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<List<Role>> GetRoleListAsync();
        public Task<Role> GetRoleByIdAsync(int roleId);
        public Task DeleteRoleAsync(int roleId);
        public Task<Role> CreateRoleAsync(Role role);
        public Task<Role> UpdateRoleAsync(Role role);
    }
}

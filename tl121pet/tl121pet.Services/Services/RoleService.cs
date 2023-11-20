using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class RoleService : IRoleService
    {
        private DataContext _dataContext;
        public RoleService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Role>> GetRoleListAsync()
        {
            return await _dataContext.Roles.ToListAsync();
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            await CheckRoleExistsByName(role);

            _dataContext.Roles.Add(role);
            await _dataContext.SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var modifiedRole = await GetRoleByIdAsync(role.RoleId);
            await CheckRoleExistsByName(role);
            await AdminRoleChangeChecking(await GetRoleNameById(role.RoleId));

            _dataContext.Entry(modifiedRole).CurrentValues.SetValues(role);
            await _dataContext.SaveChangesAsync();
            return role;
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            Role role = await GetRoleByIdAsync(roleId);
            await AdminRoleChangeChecking(role.RoleName);
            _dataContext.Roles.Remove(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            return await _dataContext.Roles.FindAsync(roleId) ?? throw new DataFoundException("Role not found");
        }

        private async Task CheckRoleExistsByName(Role role)
        {
            var examRole = await _dataContext.Roles.Where(r => r.RoleName == role.RoleName).FirstOrDefaultAsync();
            if (examRole != null)
                throw new LogicException("A Role with this name exists");
        }

        private async Task AdminRoleChangeChecking(string name)
        {
            if (name == "Admin")
                throw new LogicException("Role \"Admin\" is system Role. You can not change this Role or Create It manually");
        }

        private async Task<string> GetRoleNameById(int roleId)
        {
            Role role = await _dataContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == roleId);
            return role.RoleName;
        }
    }
}

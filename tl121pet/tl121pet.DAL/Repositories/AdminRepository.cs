using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _dataContext;

        public AdminRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task CreateRoleAsync(Role role)
        {
            _dataContext.Roles.Add(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            if (user != null)
            {
                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            Role role = _dataContext.Roles.Find(roleId);
            _dataContext.Roles.Remove(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(long userId)
        {
            User user = _dataContext.Users.Find(userId);
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Role>> GetRoleListAsync()
        {
            return await _dataContext.Roles.ToListAsync();
        }

        public async Task<string> GetRoleNameByIdAsync(int id)
        {
            Role role = await _dataContext.Roles.FindAsync(id);
            return role.RoleName;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<List<User>> GetUserListAsync()
        {
            return await _dataContext.Users.Include(p => p.Role).ToListAsync();
        }

        public async Task<List<ProjectTeam>> GetUserProjectsAsync(long userId)
        {
            List<ProjectTeam> usersProjects = new List<ProjectTeam>();

            var projects = (
                from proj in _dataContext.ProjectTeams
                join usrproj in _dataContext.UserProjects on proj.ProjectTeamId equals usrproj.ProjectTeamId
                where usrproj.UserId == userId
                select proj
            ).ToListAsync();

            foreach (var proj in await projects) 
            { 
                usersProjects.Add(proj);
            }

            return usersProjects;
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _dataContext.Roles.Update(role);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }
    }
}

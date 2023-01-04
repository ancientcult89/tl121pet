using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IAdminRepository
    {
        public User? GetUserByEmail(string email);
        public string GetRoleNameById(int id);
        public int GetRoleIdByName(string roleName);
        public User? GetUserById(long id);
        public List<ProjectTeam> GetUserProjects(long userId);
        public void UpdateRole(Role role);
        public void CreateRole(Role role);
        public void DeleteRole(int roleId);
        public void CreateUser(User user);
        public void UpdateUser(User user);
        public void DeleteUser(long userId);
        public List<User> GetUserList();
        public List<Role> GetRoleList();
    }
}

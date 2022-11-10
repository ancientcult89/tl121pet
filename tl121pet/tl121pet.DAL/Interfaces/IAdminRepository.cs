using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IAdminRepository
    {
        public User GetUserByEmail(string email);
        public void CreateUser(User user);
        public void UpdateRole(Role role);
        public void CreateRole(Role role);
        public void DeleteRole(int roleId);
    }
}

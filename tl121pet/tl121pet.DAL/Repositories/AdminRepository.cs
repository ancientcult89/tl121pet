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

        public void CreateRole(Role role)
        {
            _dataContext.Roles.Add(role);
            _dataContext.SaveChanges();
        }

        public void CreateUser(User user)
        {
            if (user != null)
            {
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();
            }
        }

        public void DeleteRole(int roleId)
        {
            Role role = _dataContext.Roles.Find(roleId);
            _dataContext.Roles.Remove(role);
            _dataContext.SaveChanges();
        }

        public User? GetUserByEmail(string email)
        {
            return _dataContext.Users.Where(u => u.Email == email).FirstOrDefault();
        }

        public void UpdateRole(Role role)
        {
            _dataContext.Roles.Update(role);
            _dataContext.SaveChanges();
        }
    }
}

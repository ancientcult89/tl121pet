using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void CreateUser(User user)
        {
            if (user != null)
            { 
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();
            }
        }

        public User? GetUserByEmail(string email)
        {
            return _dataContext.Users.Where(u => u.Email == email).FirstOrDefault();
        }
    }
}

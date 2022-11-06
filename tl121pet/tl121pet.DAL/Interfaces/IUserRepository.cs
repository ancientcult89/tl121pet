using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IUserRepository
    {
        public User GetUserByEmail(string email);
        public void CreateUser(User user);
    }
}

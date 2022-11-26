using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IPersonService
    {
        public List<Person> GetPeople();
        public List<PersonInitials> GetInitials();
    }
}

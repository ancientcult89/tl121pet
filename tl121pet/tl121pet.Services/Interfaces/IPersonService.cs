using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IPersonService
    {
        public Task<List<Person>> GetPeopleAsync();
        public Task<List<PersonInitials>> GetInitialsAsync();
    }
}

using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IPeopleRepository
    {
        public Task CreatePersonAsync(Person person);
        public Task UpdatePersonAsync(Person person);
        public Task DeletePersonAsync(long id);
        public Task<List<Person>> GetAllPeopleAsync();
        public Task<List<Person>> GetPeopleFilteredByProjectAsync(long projectTeam);
        public Task<Person> GetPersonAsync(long id);
    }
}

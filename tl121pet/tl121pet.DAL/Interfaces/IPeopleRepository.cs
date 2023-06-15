using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IPeopleRepository
    {
        public Task CreatePersonAsync(Person person);
        public Task UpdatePersonAsync(Person person);
        public Task DeletePersonAsync(long id);
        //TODO: проверить на дублирование
        public Task<List<Person>> GetPeopleAsync();
        public Task<List<Person>> GetPeopleFilteredByProjectAsync(long projectTeam);
        public Task<Person> GetPersonAsync(long id);
    }
}

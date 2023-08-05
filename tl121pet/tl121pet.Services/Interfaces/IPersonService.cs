using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IPersonService
    {
        public Task<List<Person>> GetPeopleAsync();
        public Task<List<PersonInitials>> GetInitialsAsync();
        public Task<List<Grade>> GetAllGradesAsync();
        public Task<string> GetGradeNameAsync(long id);

        public Task<Grade> CreateGradeAsync(Grade grade);
        public Task<Grade> UpdateGradeAsync(Grade grade);
        public Task DeleteGradeAsync(long id);
        public Task<Grade> GetGradeByIdAsync(long id);

        public Task CreatePersonAsync(Person person);
        public Task UpdatePersonAsync(Person person);
        public Task DeletePersonAsync(long id);
        public Task<List<Person>> GetAllPeopleAsync();
        public Task<List<Person>> GetPeopleFilteredByProjectAsync(long projectTeam);
        public Task<Person> GetPersonAsync(long id);
        public Task<List<Person>> GetPeopleWithGradeAsync();
    }
}

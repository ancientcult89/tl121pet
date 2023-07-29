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

        public Task CreateGradeAsync(Grade grade);
        public Task UpdateGradeAsync(Grade grade);
        public Task DeleteGradeAsync(long id);
        public Task<Grade> GetGradeByIdAsync(long id);
    }
}

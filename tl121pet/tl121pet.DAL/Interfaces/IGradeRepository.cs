using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IGradeRepository
    {
        public Task<List<Grade>> GetAllGradesAsync();
        public Task<string> GetGradeNameAsync(long id);

        public Task CreateGradeAsync(Grade grade);
        public Task UpdateGradeAsync(Grade grade);
        public Task DeleteGradeAsync(long id);
    }
}

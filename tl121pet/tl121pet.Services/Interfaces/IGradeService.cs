using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IGradeService
    {
        public Task<List<Grade>> GetAllGradesAsync();
        public Task<Grade> CreateGradeAsync(Grade grade);
        public Task<Grade> UpdateGradeAsync(Grade grade);
        public Task DeleteGradeAsync(long id);
        public Task<Grade> GetGradeByIdAsync(long id);
    }
}

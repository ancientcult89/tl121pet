using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private DataContext _dataContext;
        public GradeRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _dataContext.Grades.ToListAsync();
        }

        public async Task<string> GetGradeNameAsync(long id)
        {
            Grade selectedGrade = await _dataContext.Grades.FindAsync(id);
            return selectedGrade.GradeName ?? "not found";
        }

        public async Task CreateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Update(grade);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteGradeAsync(long id)
        {
            var gradeToDelete = _dataContext.Grades.Find(id);
            _dataContext.Grades.Remove(gradeToDelete);
            await _dataContext.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class GradeService : IGradeService
    {
        private DataContext _dataContext;
        public GradeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _dataContext.Grades.ToListAsync();
        }
        public async Task<Grade> CreateGradeAsync(Grade grade)
        {
            await CheckGradeExists(grade);

            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(Grade grade)
        {
            await CheckGradeExists(grade);

            _dataContext.Grades.Update(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task DeleteGradeAsync(long id)
        {
            var gradeToDelete = _dataContext.Grades.Find(id);
            if (gradeToDelete == null)
                throw new Exception("Grade to delete not found");
            _dataContext.Grades.Remove(gradeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(long id)
        {
            return await _dataContext.Grades.FindAsync(id) ?? throw new Exception("Grade not found");
        }

        private async Task CheckGradeExists(Grade grade)
        {
            var examGrade = await _dataContext.Grades.Where(g => g.GradeName == grade.GradeName).FirstOrDefaultAsync();
            if (examGrade != null)
                throw new Exception("An Grade with this name exists");
        }
    }
}

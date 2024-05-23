using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class GradeService (DataContext dataContext) : IGradeService
    {
        private DataContext _dataContext = dataContext;

        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _dataContext.Grades.ToListAsync();
        }
        public async Task<Grade> CreateGradeAsync(Grade grade)
        {
            await CheckGradeExistsByName(grade.GradeName);
            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(Grade grade)
        {
            var modifiedGrade = await GetGradeByIdAsync(grade.GradeId);
            await CheckGradeExistsByName(grade.GradeName);

            _dataContext.Entry(modifiedGrade).CurrentValues.SetValues(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task DeleteGradeAsync(long id)
        {
            var gradeToDelete = await GetGradeByIdAsync(id);
            _dataContext.Grades.Remove(gradeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(long id)
        {
            return await _dataContext.Grades.FindAsync(id) ?? throw new DataFoundException("Grade not found");
        }

        private async Task CheckGradeExistsByName(string gradeName)
        {
            var examGrade = await _dataContext.Grades.Where(g => g.GradeName == gradeName).FirstOrDefaultAsync();
            if (examGrade != null)
                throw new LogicException("A Grade with this name exists");
        }
    }
}

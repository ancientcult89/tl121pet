﻿using Microsoft.EntityFrameworkCore;
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
            await CheckGradeExistsByName(grade.GradeName);
            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(Grade grade)
        {
            await CheckGradeExistsByName(grade.GradeName);
            await CheckGradeExistsById(grade.GradeId);

            _dataContext.Grades.Update(grade);
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
            return await _dataContext.Grades.FindAsync(id) ?? throw new Exception("Grade not found");
        }

        private async Task CheckGradeExistsByName(string gradeName)
        {
            var examGrade = await _dataContext.Grades.AsNoTracking().Where(g => g.GradeName == gradeName).FirstOrDefaultAsync();
            if (examGrade != null)
                throw new Exception("A Grade with this name exists");
        }

        private async Task CheckGradeExistsById(long gradeId)
        {
            var examGrade = await _dataContext.Grades.AsNoTracking().Where(g => g.GradeId == gradeId).FirstOrDefaultAsync();
            if (examGrade == null)
                throw new Exception("Grade not found");
        }
    }
}

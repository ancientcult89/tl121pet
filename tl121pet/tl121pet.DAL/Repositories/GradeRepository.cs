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
        public List<Grade> GetAllGrades()
        {
            return _dataContext.Grades.ToList();
        }

        public string GetGradeName(long id)
        {
            return _dataContext.Grades.Find(id).GradeName ?? "not found";
        }

        public void CreateGrade(Grade grade)
        {
            _dataContext.Grades.Add(grade);
            _dataContext.SaveChanges();
        }

        public void UpdateGrade(Grade grade)
        {
            _dataContext.Grades.Update(grade);
            _dataContext.SaveChanges();
        }

        public void DeleteGrade(long id)
        {
            var gradeToDelete = _dataContext.Grades.Find(id);
            _dataContext.Grades.Remove(gradeToDelete);
            _dataContext.SaveChanges();
        }
    }
}

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
    }
}

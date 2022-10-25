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
        public List<Grade> GetAllGrades()
        {
            return _dataContext.Grades.ToList();
        }
    }
}

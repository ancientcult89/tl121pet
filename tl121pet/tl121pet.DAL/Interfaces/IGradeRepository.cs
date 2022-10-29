using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IGradeRepository
    {
        public List<Grade> GetAllGrades();
    }
}

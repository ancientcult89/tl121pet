using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IGradeRepository
    {
        public List<Grade> GetAllGrades();
        public string GetGradeName(long id);

        public void CreateGrade(Grade grade);
        public void UpdateGrade(Grade grade);
        public void DeleteGrade(long id);
    }
}

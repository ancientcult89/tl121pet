//сетка грейда
//название
//ставка
namespace tl121pet.Entities.Models
{
    public class Grade
    {
        public long GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public long SalaryId { get; set; }
    }
}

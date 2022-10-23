//описание скиллов
//тип скилла
//группа скилла - нужно для оценки, когда необходимо будет набрать, например 3/4 баллов в группе
//
namespace tl121pet.Entities.Models
{
    public class Skill
    {
        public long SkillId { get; set; }
        public string SkillsDescription { get; set; } = string.Empty;
        public int SkillTypeId { get; set; }
        public SkillType? SkillType { get; set; }

        public long SkillGroupId { get; set; }
        public SkillGroup? SkillGroup { get; set; }
    }
}

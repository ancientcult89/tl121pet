//нужно при реализации разграничения пользователей, от того, кто залогинелся будут отображаться только разработчики определённой команды
//имя проекта
//описание
namespace tl121pet.Entities.Models
{
    public class ProjectTeam
    {
        public long ProjectTeamId { get; set; }
        public string ProjectTeamName { get; set; }
    }
}

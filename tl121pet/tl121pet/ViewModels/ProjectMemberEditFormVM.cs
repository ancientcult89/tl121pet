using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class ProjectMemberEditFormVM : SimpleEditFormVM<Person>
    {
        public List<ProjectTeam>? ProjectTeams { get; set; }
        public long NewProjectTeamId { get; set; }
    }
}

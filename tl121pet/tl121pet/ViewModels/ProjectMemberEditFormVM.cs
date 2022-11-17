using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class ProjectMemberEditFormVM : SimpleEditFormVM<long>
    {
        public List<ProjectTeam>? ProjectTeams { get; set; }
    }
}

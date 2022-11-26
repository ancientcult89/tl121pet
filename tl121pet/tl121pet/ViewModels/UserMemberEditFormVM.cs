using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class UserMemberEditFormVM : SimpleEditFormVM<User>
    {
        public List<ProjectTeam>? ProjectTeams { get; set; }
        public long NewProjectTeamId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class ProjectMemberEditFormVM : SimpleEditFormVM<Person>
    {
        public List<ProjectTeam>? ProjectTeams { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Required")]
        public long NewProjectTeamId { get; set; }
    }
}

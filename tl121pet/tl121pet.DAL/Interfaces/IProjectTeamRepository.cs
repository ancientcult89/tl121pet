using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IProjectTeamRepository
    {
        public List<ProjectTeam> GetAllTeams();
    }
}

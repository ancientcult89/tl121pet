using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IProjectTeamRepository
    {
        public List<ProjectTeam> GetAllTeams();
        public ProjectTeam GetProjectTeamById(long id);
        public void CreateProjectTeam(ProjectTeam pt);
        public void UpdateProjectTeam(ProjectTeam pt);
        public void DeleteProjectTeam(long id);
        public string GetPersonsProjects(long id);
    }
}

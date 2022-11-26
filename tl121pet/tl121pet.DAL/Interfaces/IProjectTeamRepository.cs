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
        public string GetUserProjects(long userId);
        public List<ProjectTeam> GetPersonMembership(long id);
        public List<ProjectTeam> GetUserMembership(long id);
        public void DeletePersonMembership(long userId, long projectTeamId);
        public void AddPersonMembership(long userId, long projectTeamId);
        public void DeleteUserMembership(long userId, long projectTeamId);
        public void AddUserMembership(long userId, long projectTeamId);
    }
}

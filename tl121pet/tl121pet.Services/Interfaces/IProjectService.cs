using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<List<ProjectTeam>> GetFilteredProjectsAsync();
        public Task<List<ProjectTeam>> GetAllTeamsAsync();
        public Task<ProjectTeam> GetProjectTeamByIdAsync(long id);
        public Task<ProjectTeam> CreateProjectTeamAsync(ProjectTeam pt);
        public Task<ProjectTeam> UpdateProjectTeamAsync(ProjectTeam pt);
        public Task DeleteProjectTeamAsync(long id);
        public Task<string> GetPersonsProjectsAsync(long id);
        public Task<string> GetUserProjectsNameAsync(long userId);
        public Task<List<ProjectTeam>> GetPersonMembershipAsync(long id);
        public Task<List<ProjectTeam>> GetUserMembershipAsync(long id);
        public Task DeletePersonMembershipAsync(long userId, long projectTeamId);
        public Task AddPersonMembershipAsync(long userId, long projectTeamId);
        public Task DeleteUserMembershipAsync(long userId, long projectTeamId);
        public Task AddUserMembershipAsync(long userId, long projectTeamId);
    }
}

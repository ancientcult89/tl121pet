using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;

        public ProjectService(DataContext dataContext, IAuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }
        public async Task<List<ProjectTeam>> GetFilteredProjectsAsync()
        {
            List<ProjectTeam> filteredProjects = new List<ProjectTeam>();

            long? userId = _authService.GetMyUserId();

            var projects = from up in _dataContext.UserProjects
                           join pr in _dataContext.ProjectTeams on up.ProjectTeamId equals pr.ProjectTeamId
                           where up.UserId == userId
                           select pr;

            foreach (ProjectTeam project in projects)
            {
                filteredProjects.Add(project);
            }

            return filteredProjects;
        }
    }
}

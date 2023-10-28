using Microsoft.EntityFrameworkCore;
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

        [Obsolete]
        public async Task<List<ProjectTeam>> GetFilteredProjectsAsync(long? userId)
        {
            List<ProjectTeam> filteredProjects = new List<ProjectTeam>();

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

        public async Task<List<ProjectTeam>> GetAllTeamsAsync()
        {
            return await _dataContext.ProjectTeams.ToListAsync();
        }

        public async Task<ProjectTeam> GetProjectTeamByIdAsync(long id)
        {
            return await _dataContext.ProjectTeams.FindAsync(id) ?? new ProjectTeam();
        }

        public async Task DeleteProjectTeamAsync(long id)
        {
            ProjectTeam pt = _dataContext.ProjectTeams.Find(id);
            _dataContext.ProjectTeams.Remove(pt);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<ProjectTeam> CreateProjectTeamAsync(ProjectTeam pt)
        {
            _dataContext.ProjectTeams.Add(pt);
            await _dataContext.SaveChangesAsync();
            return pt;
        }

        public async Task<ProjectTeam> UpdateProjectTeamAsync(ProjectTeam pt)
        {
            _dataContext.ProjectTeams.Update(pt);
            await _dataContext.SaveChangesAsync();
            return pt;
        }

        public async Task<string> GetPersonsProjectsAsStringAsync(long id)
        {
            string projectsList = "";
            List<ProjectTeam> projectTeamsList = await GetPersonMembershipAsync(id);

            foreach (ProjectTeam pm in projectTeamsList)
            {
                projectsList += $"{pm.ProjectTeamName}; ";
            }
            return projectsList;
        }

        public async Task<List<ProjectTeam>> GetPersonMembershipAsync(long id)
        {
            var selectedTeams = await (
                from pt in _dataContext.ProjectTeams
                join pm in _dataContext.ProjectMembers on pt.ProjectTeamId equals pm.ProjectTeamId
                where pm.PersonId == id
                select pt
            ).ToListAsync();

            return selectedTeams;
        }

        public async Task DeletePersonMembershipAsync(long userId, long projectTeamId)
        {
            ProjectMember pm = await _dataContext.ProjectMembers
                .Where(p => p.ProjectTeamId == projectTeamId && p.PersonId == userId)
                .FirstOrDefaultAsync();
            _dataContext.ProjectMembers.Remove(pm);
            _dataContext.SaveChanges();
        }

        public async Task<ProjectMember> AddPersonMembershipAsync(long userId, long projectTeamId)
        {
            ProjectMember pm = new ProjectMember()
            {
                PersonId = userId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.ProjectMembers.Add(pm);
            await _dataContext.SaveChangesAsync();
            return pm;
        }

        public async Task<List<ProjectTeam>> GetUserMembershipAsync(long id)
        {
            List<ProjectTeam> result = new List<ProjectTeam>();
            var selectedTeams = (
                from pt in _dataContext.ProjectTeams
                join up in _dataContext.UserProjects on pt.ProjectTeamId equals up.ProjectTeamId
                where up.UserId == id
                select pt
            ).ToListAsync();

            foreach (ProjectTeam pm in await selectedTeams)
            {
                result.Add(pm);
            }

            return result;
        }

        public async Task DeleteUserMembershipAsync(long userId, long projectTeamId)
        {
            UserProject up = await _dataContext.UserProjects
                .Where(p => p.ProjectTeamId == projectTeamId && p.UserId == userId)
                .FirstOrDefaultAsync();
            _dataContext.UserProjects.Remove(up);
            await _dataContext.SaveChangesAsync();
        }

        public async Task AddUserMembershipAsync(long userId, long projectTeamId)
        {
            UserProject up = new UserProject()
            {
                UserId = userId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.UserProjects.Add(up);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<string> GetUserProjectsNameAsync(long userId)
        {
            string projectsList = "";
            List<UserProject> userMemberList = await _dataContext.UserProjects
                .Include(p => p.ProjectTeam)
                .Where(pm => pm.UserId == userId)
                .OrderBy(pm => pm.ProjectTeam.ProjectTeamName)
                .ToListAsync();
            foreach (UserProject up in userMemberList)
            {
                projectsList += $"{up.ProjectTeam.ProjectTeamName}; ";
            }
            return projectsList;
        }
    }
}

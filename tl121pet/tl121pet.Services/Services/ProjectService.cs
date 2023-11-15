using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;

        public ProjectService(DataContext dataContext)
        {
            _dataContext = dataContext;
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
            return await _dataContext.ProjectTeams.FindAsync(id) ?? throw new DataFoundException("Project not found");
        }

        public async Task DeleteProjectTeamAsync(long id)
        {
            ProjectTeam pt = await GetProjectTeamByIdAsync(id);
            _dataContext.ProjectTeams.Remove(pt);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<ProjectTeam> CreateProjectTeamAsync(ProjectTeam pt)
        {
            await CheckProjectsExistsByName(pt);
            _dataContext.ProjectTeams.Add(pt);
            await _dataContext.SaveChangesAsync();
            return pt;
        }

        public async Task<ProjectTeam> UpdateProjectTeamAsync(ProjectTeam pt)
        {
            var modifiedProject = await GetProjectTeamByIdAsync(pt.ProjectTeamId);
            await CheckProjectsExistsByName(pt);
            _dataContext.Entry(modifiedProject).CurrentValues.SetValues(pt);
            await _dataContext.SaveChangesAsync();
            return pt;
        }

        public async Task<ProjectMember> AddPersonMembershipAsync(long personId, long projectTeamId)
        {
            await CheckExistsPersonProjectsFoDuplicates(personId, projectTeamId);
            ProjectMember pm = new ProjectMember()
            {
                PersonId = personId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.ProjectMembers.Add(pm);
            await _dataContext.SaveChangesAsync();
            return pm;
        }

        public async Task<List<ProjectTeam>> GetPersonMembershipAsync(long personId)
        {
            var selectedTeams = await (
                from pt in _dataContext.ProjectTeams
                join pm in _dataContext.ProjectMembers on pt.ProjectTeamId equals pm.ProjectTeamId
                where pm.PersonId == personId
                select pt
            ).ToListAsync();

            return selectedTeams;
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

        public async Task DeletePersonMembershipAsync(long userId, long projectTeamId)
        {
            await CheckExistsPersonProjects(userId, projectTeamId);

            ProjectMember pm = await _dataContext.ProjectMembers
                .Where(p => p.ProjectTeamId == projectTeamId && p.PersonId == userId)
                .FirstOrDefaultAsync();
            _dataContext.ProjectMembers.Remove(pm);
            _dataContext.SaveChanges();
        }

        public async Task<UserProject> AddUserMembershipAsync(long userId, long projectTeamId)
        {
            await CheckExistsUserProjectsFoDuplicates(userId, projectTeamId);
            UserProject userProject = new UserProject()
            {
                UserId = userId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.UserProjects.Add(userProject);
            await _dataContext.SaveChangesAsync();

            return userProject;
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
            await CheckExistsUserProjects(userId, projectTeamId);

            UserProject up = await _dataContext.UserProjects
                .Where(p => p.ProjectTeamId == projectTeamId && p.UserId == userId)
                .FirstOrDefaultAsync();
            _dataContext.UserProjects.Remove(up);
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

        private async Task CheckProjectsExistsByName(ProjectTeam project)
        {
            var examProject = await _dataContext.ProjectTeams.SingleOrDefaultAsync(r => r.ProjectTeamName == project.ProjectTeamName);
            if(examProject != null)
                throw new LogicException("A Project with same name is already exists");
        }

        private async Task CheckExistsPersonProjectsFoDuplicates(long personId, long projectTeamId)
        {
            var result = await _dataContext.ProjectMembers
                .Where(pm => pm.PersonId == personId && pm.ProjectTeamId == projectTeamId)
                .FirstOrDefaultAsync();
            if(result != null)
                throw new LogicException("The Project is already used");
        }
        private async Task<ProjectMember> CheckExistsPersonProjects(long userId, long projectTeamId)
        {
            var result = await _dataContext.ProjectMembers
                .Where(pm => pm.PersonId == userId && pm.ProjectTeamId == projectTeamId)
                .FirstOrDefaultAsync();
            if (result == null)
                throw new DataFoundException("The Persons Project not exist");

            return result;
        }

        private async Task CheckExistsUserProjectsFoDuplicates(long userId, long projectTeamId)
        {
            var result = await _dataContext.UserProjects
                .Where(pm => pm.UserId == userId && pm.ProjectTeamId == projectTeamId)
                .FirstOrDefaultAsync();
            if (result != null)
                throw new LogicException("The Project is already used");
        }

        private async Task<UserProject> CheckExistsUserProjects(long userId, long projectTeamId)
        {
            var result = await _dataContext.UserProjects
                .Where(pm => pm.UserId == userId && pm.ProjectTeamId == projectTeamId)
                .FirstOrDefaultAsync();
            if (result == null)
                throw new DataFoundException("The User Project not exist");

            return result;
        }
    }
}

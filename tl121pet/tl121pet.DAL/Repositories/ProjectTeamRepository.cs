using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class ProjectTeamRepository : IProjectTeamRepository
    {
        private DataContext _dataContext;
        public ProjectTeamRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public List<ProjectTeam> GetAllTeams()
        {
            return _dataContext.ProjectTeams.ToList();
        }

        public ProjectTeam GetProjectTeamById(long id)
        {
            return _dataContext.ProjectTeams.Find(id) ?? new ProjectTeam();
        }

        public void DeleteProjectTeam(long id)
        { 
            ProjectTeam pt = _dataContext.ProjectTeams.Find(id);
            _dataContext.ProjectTeams.Remove(pt);
            _dataContext.SaveChanges();
        }

        public void CreateProjectTeam(ProjectTeam pt)
        { 
            _dataContext.ProjectTeams.Add(pt);
            _dataContext.SaveChanges();
        }

        public void UpdateProjectTeam(ProjectTeam pt)
        {
            _dataContext.ProjectTeams.Update(pt);
            _dataContext.SaveChanges();
        }

        public string GetPersonsProjects(long id)
        {
            string projectsList = "";
            List<ProjectMember> projectMemberList = _dataContext.ProjectMembers
                .Include(p => p.ProjectTeam)
                .Where(pm => pm.PersonId == id)
                .OrderBy(pm => pm.ProjectTeam.ProjectTeamName)
                .ToList();
            foreach (ProjectMember pm in projectMemberList) {
                projectsList += $"{pm.ProjectTeam.ProjectTeamName}; ";
            }
            return projectsList;
        }

        public List<ProjectTeam> GetPersonMembership(long id)
        {
            List<ProjectTeam> result = new List<ProjectTeam>();
            var selectedTeams = from pt in _dataContext.ProjectTeams
                                join pm in _dataContext.ProjectMembers on pt.ProjectTeamId equals pm.ProjectTeamId
                                where pm.PersonId == id
                                select pt;

            foreach (ProjectTeam pm in selectedTeams) {
                result.Add(pm);
            }

            return result;
        }

        public void DeletePersonMembership(long userId, long projectTeamId)
        { 
            ProjectMember pm = _dataContext.ProjectMembers
                .Where(p => p.ProjectTeamId == projectTeamId && p.PersonId == userId)
                .FirstOrDefault();
            _dataContext.ProjectMembers.Remove(pm);
            _dataContext.SaveChanges();
        }

        public void AddPersonMembership(long userId, long projectTeamId)
        {
            ProjectMember pm = new ProjectMember() { 
                PersonId = userId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.ProjectMembers.Add(pm);
            _dataContext.SaveChanges();
        }

        public List<ProjectTeam> GetUserMembership(long id)
        {
            List<ProjectTeam> result = new List<ProjectTeam>();
            var selectedTeams = from pt in _dataContext.ProjectTeams
                                join up in _dataContext.UserProjects on pt.ProjectTeamId equals up.ProjectTeamId
                                where up.UserId == id
                                select pt;

            foreach (ProjectTeam pm in selectedTeams)
            {
                result.Add(pm);
            }

            return result;
        }

        public void DeleteUserMembership(long userId, long projectTeamId)
        {
            UserProject up = _dataContext.UserProjects
                .Where(p => p.ProjectTeamId == projectTeamId && p.UserId == userId)
                .FirstOrDefault();
            _dataContext.UserProjects.Remove(up);
            _dataContext.SaveChanges();
        }

        public void AddUserMembership(long userId, long projectTeamId)
        {
            UserProject up = new UserProject()
            {
                UserId = userId,
                ProjectTeamId = projectTeamId
            };
            _dataContext.UserProjects.Add(up);
            _dataContext.SaveChanges();
        }

        public string GetUserProjects(long userId)
        {
            string projectsList = "";
            List<UserProject> userMemberList = _dataContext.UserProjects
                .Include(p => p.ProjectTeam)
                .Where(pm => pm.UserId == userId)
                .OrderBy(pm => pm.ProjectTeam.ProjectTeamName)
                .ToList();
            foreach (UserProject up in userMemberList)
            {
                projectsList += $"{up.ProjectTeam.ProjectTeamName}; ";
            }
            return projectsList;
        }
    }
}

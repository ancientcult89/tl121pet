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
    }
}

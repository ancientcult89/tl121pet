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
    }
}

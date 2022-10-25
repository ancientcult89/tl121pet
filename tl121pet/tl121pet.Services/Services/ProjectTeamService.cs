using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class ProjectTeamService : IProjectTeamService
    {
        private DataContext _dataContext;
        public ProjectTeamService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public List<ProjectTeam> GetAllTeams()
        {
            return _dataContext.ProjectTeams.ToList();
        }
    }
}

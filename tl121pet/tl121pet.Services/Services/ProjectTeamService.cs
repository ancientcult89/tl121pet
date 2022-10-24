using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class ProjectTeamService : IProjectTeamService
    {
        private IDataRepository _repository;
        private DataContext _dataContext;
        public ProjectTeamService(IDataRepository repository, DataContext dataContext)
        {
            _repository = repository;
            _dataContext = dataContext;
        }
        public List<ProjectTeam> GetAllTeams()
        {
            return _dataContext.ProjectTeams.ToList();
        }
    }
}

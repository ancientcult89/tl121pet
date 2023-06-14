using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<List<ProjectTeam>> GetFilteredProjectsAsync();
    }
}

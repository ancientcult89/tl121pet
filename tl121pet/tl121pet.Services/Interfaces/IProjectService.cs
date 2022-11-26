using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IProjectService
    {
        public List<ProjectTeam> GetFilteredProjects();
    }
}

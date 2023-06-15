using tl121pet.Entities.DTO;

namespace tl121pet.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetTaskListAsync(long? personId);
    }
}

using tl121pet.Entities.DTO;

namespace tl121pet.Services.Interfaces
{
    public interface ITaskService
    {
        List<TaskDTO> GetTaskList(long? personId);
    }
}

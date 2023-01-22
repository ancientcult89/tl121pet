using tl121pet.Entities.DTO;

namespace tl121pet.ViewModels
{
    public class TaskListVM
    {
        public List<TaskDTO> Tasks { get; set; }
        public long? PersonId { get; set; }
    }
}

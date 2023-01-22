using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Services.Interfaces;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class TaskController : Controller
    {
        private ITaskService _taskService;
        private IMeetingRepository _meetingRepository;
        public TaskController(ITaskService taskService, IMeetingRepository meetingRepository)
        {
            _taskService = taskService;
            _meetingRepository = meetingRepository;
        }
        public IActionResult TaskList(long? personId = null)
        {
            List<TaskDTO> tasks = _taskService.GetTaskList(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId});
        }

        [HttpPost]
        public IActionResult CompleteGoal(Guid goalId, long? personId, string CompleteDescription)
        {
            if (ModelState.IsValid)
            { 
                _meetingRepository.CompleteGoal(goalId, CompleteDescription);
            }

            List<TaskDTO> tasks = _taskService.GetTaskList(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId });
        }
    }
}

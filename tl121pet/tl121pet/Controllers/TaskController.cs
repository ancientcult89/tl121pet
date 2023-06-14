using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Services.Interfaces;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private ITaskService _taskService;
        private IMeetingRepository _meetingRepository;
        public TaskController(ITaskService taskService, IMeetingRepository meetingRepository)
        {
            _taskService = taskService;
            _meetingRepository = meetingRepository;
        }
        public async Task<IActionResult> TaskList(long? personId = null)
        {
            List<TaskDTO> tasks = await _taskService.GetTaskListAsync(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId});
        }

        [HttpPost]
        public async Task<IActionResult> CompleteGoal(Guid goalId, long? personId, string CompleteDescription)
        {
            if (ModelState.IsValid)
            { 
                _meetingRepository.CompleteGoal(goalId, CompleteDescription);
            }

            List<TaskDTO> tasks = await _taskService.GetTaskListAsync(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId });
        }
    }
}

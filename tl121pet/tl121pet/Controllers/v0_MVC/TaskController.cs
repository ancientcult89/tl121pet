using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Services.Application;
using tl121pet.Services.Interfaces;
using tl121pet.ViewModels;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize]
    public class TaskController : Controller
    {

        private IMeetingService _meetingService;
        private OneToOneApplication _application;
        public TaskController(IMeetingService meetingService, OneToOneApplication application)
        {
            _meetingService = meetingService;
            _application = application;
        }
        public async Task<IActionResult> TaskList(long? personId = null)
        {
            List<TaskDTO> tasks = await _application.GetTaskListAsync(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId});
        }

        [HttpPost]
        public async Task<IActionResult> CompleteGoal(Guid goalId, long? personId)
        {
            if (ModelState.IsValid)
            { 
                await _meetingService.CompleteGoalAsync(goalId);
            }

            List<TaskDTO> tasks = await _application.GetTaskListAsync(personId);

            return View("TaskList", new TaskListVM { Tasks = tasks, PersonId = personId });
        }
    }
}

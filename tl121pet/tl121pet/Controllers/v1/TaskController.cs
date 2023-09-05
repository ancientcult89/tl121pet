using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TaskController : ApiController
    {
        private ITaskService _taskService;
        private IMeetingService _meetingService;
        public TaskController(ITaskService taskService, IMeetingService meetingService)
        {
            _taskService = taskService;
            _meetingService = meetingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDTO>>> GetTaskList(long? personId = null)
        {
            return await  _taskService.GetTaskListAsync(personId);
        }

        [HttpPost]
        public async Task<ActionResult> CompleteTask(TaskCompleteRequestDTO request)
        {
            await _meetingService.CompleteGoalAsync(request.GoalId, request.CompleteDescription);
            return Ok();
        }
    }
}

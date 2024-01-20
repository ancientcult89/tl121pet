using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TaskController : ApiController
    {
        private IMeetingService _meetingService;
        private IOneToOneApplication _application;
        public TaskController(IMeetingService meetingService, IOneToOneApplication application)
        {
            _meetingService = meetingService;
            _application = application;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDTO>>> GetTaskList(Guid? currentMeetingId, long? personId = null)
        {
            return await  _application.GetTaskListAsync(personId, currentMeetingId);
        }

        [HttpPost]
        public async Task<ActionResult> CompleteTask(TaskCompleteRequestDTO request)
        {
            await _meetingService.CompleteGoalAsync(request.GoalId);
            return Ok();
        }
    }
}

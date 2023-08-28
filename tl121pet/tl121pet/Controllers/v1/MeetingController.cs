using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MeetingController : ApiController
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Meeting>>> GetMeetingList(long? personId = null)
        {
            return await _meetingService.GetMeetingsAsync(personId);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeetingById(Guid id)
        {
            return await _meetingService.GetMeetingByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Meeting>> CreateMeeting([FromBody] Meeting newMeeting)
        {
            return await _meetingService.CreateMeetingAsync(newMeeting);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Meeting>> UpdateMeeting([FromBody] Meeting meeting)
        {
            return await _meetingService.UpdateMeetingAsync(meeting);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMeeting(Guid id)
        {
            await _meetingService.DeleteMeetingAsync(id);
            return Ok();
        }
    }
}

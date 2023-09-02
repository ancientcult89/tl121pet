using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
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
        private readonly IAutomapperMini _automapperMini;
        private readonly IOneToOneService _oneToOneService;

        public MeetingController(IMeetingService meetingService, IAutomapperMini automapperMini, IOneToOneService oneToOneService)
        {
            _meetingService = meetingService;
            _automapperMini = automapperMini;
            _oneToOneService = oneToOneService;
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
        public async Task<ActionResult<Meeting>> CreateMeeting([FromBody] MeetingDTO newMeeting)
        {
            return await _meetingService.CreateMeetingAsync(_automapperMini.MeetingDtoToEntity(newMeeting));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Meeting>> UpdateMeeting([FromBody] MeetingDTO meeting)
        {
            return await _meetingService.UpdateMeetingAsync(_automapperMini.MeetingDtoToEntity(meeting));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMeeting(Guid id)
        {
            await _meetingService.DeleteMeetingAsync(id);
            return Ok();
        }

        [HttpGet("previous")]
        public async Task<ActionResult<string>> GetPreviousMeetingNotesAndGoals(Guid meetingId, long personId)
        {
            return await _oneToOneService.GetPreviousMeetingNoteAndGoalsAsync(meetingId, personId);
        }
    }
}

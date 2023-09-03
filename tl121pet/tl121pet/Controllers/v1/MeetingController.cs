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

        #region Meeting

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

        #endregion Meeting

        #region MeetingNotes

        [HttpGet("{id}/note")]
        public async Task<ActionResult<List<MeetingNote>>> GetMeetingNotesList(Guid id)
        {
            return await _meetingService.GetMeetingNotesAsync(id);
        }

        [HttpDelete("{id}/note/{meetingNoteId}")]
        public async Task<ActionResult> DeleteNote(Guid meetingNoteId)
        {
            await _meetingService.DeleteNoteAsync(meetingNoteId);
            return Ok();
        }

        [HttpPost("{id}/note")]
        public async Task<ActionResult<MeetingNote>> CreateNote([FromBody] MeetingNoteDTO meetingNote)
        {
            return await _meetingService.AddNoteAsync(_automapperMini.MeetingNoteDtoToEntity(meetingNote));
        }

        [HttpPut("{id}/note")]
        public async Task<ActionResult<MeetingNote>> UpdateNote([FromBody] MeetingNoteDTO meetingNote)
        {
            return await _meetingService.UpdateNoteAsync(_automapperMini.MeetingNoteDtoToEntity(meetingNote));
        }

        #endregion MeetingNotes

        #region MeetingGoal
        [HttpGet("{id}/goal")]
        public async Task<ActionResult<List<MeetingGoal>>> GetMeetingGoalsList(Guid id)
        {
            return await _meetingService.GetMeetingGoalsAsync(id);
        }

        [HttpDelete("{id}/goal/{meetingGoalId}")]
        public async Task<ActionResult> DeleteGoal(Guid meetingGoalId)
        {
            await _meetingService.DeleteGoalAsync(meetingGoalId);
            return Ok();
        }

        [HttpPost("{id}/goal")]
        public async Task<ActionResult<MeetingGoal>> CreateGoal([FromBody] MeetingGoalDTO meetingGoal)
        {
            return await _meetingService.AddGoalAsync(_automapperMini.MeetingGoalDtoToEntity(meetingGoal));
        }

        [HttpPut("{id}/goal")]
        public async Task<ActionResult<MeetingGoal>> UpdateGoal([FromBody] MeetingGoalDTO meetingGoal)
        {
            return await _meetingService.UpdateGoalAsync(_automapperMini.MeetingGoalDtoToEntity(meetingGoal));
        }
        #endregion MeetingGoal
    }
}

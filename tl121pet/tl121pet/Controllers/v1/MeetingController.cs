using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MeetingController : ApiController
    {
        private readonly IMeetingService _meetingService;
        private readonly IOneToOneApplication _application;

        public MeetingController(IMeetingService meetingService, IOneToOneApplication application)
        {
            _meetingService = meetingService;
            _application = application;
        }

        #region Meeting

        [HttpGet]
        public async Task<ActionResult<List<Meeting>>> GetMeetingList(long? personId = null)
        {
            return await _application.GetMeetingsAsync(personId);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeetingById(Guid id)
        {
            return await _meetingService.GetMeetingByIdMeetingAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Meeting>> CreateMeeting([FromBody] MeetingDTO newMeeting)
        {
            return await _application.CreateMeetingAsync(newMeeting);
        }

        [HttpPost("{personId}")]
        public async Task<ActionResult<Meeting>> CreateMeetingForProcessing(long personId)
        {
            return await _application.CreateMeetingByPersonIdAsync(personId);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Meeting>> UpdateMeeting([FromBody] MeetingDTO meeting)
        {
            return await _meetingService.UpdateMeetingAsync(meeting.ToEntity());
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
            return await _application.GetPreviousMeetingNoteAndGoalsAsync(meetingId, personId);
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
            return await _meetingService.AddNoteAsync(meetingNote.ToEntity());
        }

        [HttpPut("{id}/note")]
        public async Task<ActionResult<MeetingNote>> UpdateNote([FromBody] MeetingNoteDTO meetingNote)
        {
            return await _meetingService.UpdateNoteAsync(meetingNote.ToEntity());
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
            return await _meetingService.AddGoalAsync(meetingGoal.ToEntity());
        }

        [HttpPut("{id}/goal")]
        public async Task<ActionResult<MeetingGoal>> UpdateGoal([FromBody] MeetingGoalDTO meetingGoal)
        {
            return await _meetingService.UpdateGoalAsync(meetingGoal.ToEntity());
        }
        #endregion MeetingGoal

        #region Processing
        [HttpGet("{id}/followup")]
        public async Task<ActionResult<string>> GenerateFollowUp(Guid meetingId, long personId)
        {
            return await _application.GenerateFollowUpAsync(meetingId, personId);
        }

        //TODO: Логика со старого бекенда: есть метод выше на формирование фолоуаппа, в методе ниже при отправке мы снова его будем генерировать
        //возможно имеет смысл передавать только айдишку сотрудника и сгенеренный текст фоллоуаппа
        [HttpPost("{id}/followup")]
        public async Task<ActionResult> SendFollowUp([FromBody] SendFollowUpRequestDTO request)
        {
            await _application.SendFollowUpAsync(request.MeetingId, request.PersonId);
            return Ok();
        }
        #endregion Processing
    }
}

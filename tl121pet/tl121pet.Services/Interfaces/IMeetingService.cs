using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IMeetingService
    {
        #region Meetings
        public Task<List<Meeting>> GetMeetingsByPersonAsync(List<Person> people);
        public Task<Meeting> GetMeetingByIdMeetingAsync(Guid id);
        public Task<List<Meeting>> GetMeetingsByUserIdAsync(long userId, long? personId);
        public Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId);
        public Task<Meeting> CreateMeetingAsync(Meeting m);
        public Task<Meeting> UpdateMeetingAsync(Meeting mtdto);
        public Task DeleteMeetingAsync(Guid id);
        public Task<Meeting?> GetLastOneToOneByPersonIdAsync(long personId);
        public Task<Meeting> MarkAsSendedFollowUpAndFillActualDateAsync(Guid meetingId, DateTime actualDate);
        public Task<List<TaskDTO>> GetTasksByUserId(long userId);
        #endregion Meetings

        #region MeetingNotes
        public Task<MeetingNote> AddNoteAsync(MeetingNote meetingNote);
        public Task<MeetingNote> UpdateNoteAsync(MeetingNote meetingNote);
        public Task DeleteNoteAsync(Guid id);
        public Task<List<MeetingNote>> GetMeetingNotesAsync(Guid meetingId);
        public Task<List<MeetingNote>> GetMeetingFeedbackRequiredNotesAsync(Guid id);
        #endregion MeetingNotes

        #region MeetingGoals
        public Task<MeetingGoal> AddGoalAsync(MeetingGoal meetingGoal);
        public Task<MeetingGoal> UpdateGoalAsync(MeetingGoal meetingGoal);
        public Task DeleteGoalAsync(Guid id);
        public Task<List<MeetingGoal>> GetMeetingGoalsAsync(Guid id);
        [Obsolete]
        public Task<List<MeetingGoal>> GetMeetingGoalsByPersonAsync(long personId);
        public Task CompleteGoalAsync(Guid goalId);
        #endregion MeetingGoals
    }
}

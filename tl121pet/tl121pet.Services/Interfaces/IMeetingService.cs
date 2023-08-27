using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IMeetingService
    {
        #region Meetings
        public Task<List<Meeting>> GetMeetingsAsync(long? personId);
        public Task<Meeting> GetMeetingByIdAsync(Guid id);
        public Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId);
        public Task<Meeting> CreateMeetingAsync(Meeting m);
        public Task<Meeting> UpdateMeetingAsync(Meeting mtdto);
        public Task DeleteMeetingAsync(Guid id);
        public Task<List<Meeting>> GetMeetingsByPersonIdAsync(long personId);
        public Task<Meeting?> GetLastOneToOneByPersonIdAsync(long personId);
        public Task MarkAsSendedFollowUpAsync(Guid meetingId);
        public Task<DateTime?> GetFactMeetingDateByIdAsync(Guid meetingId);
        #endregion Meetings

        #region MeetingTypes
        public Task CreateMeetingTypeAsync(MeetingType mt);
        public Task UpdateMeetingTypeAsync(MeetingType mt);
        public Task DeleteMeetingTypeAsync(int id);
        public Task<List<MeetingType>> GetAllMeetingTypesAsync();
        public Task<MeetingType> GetMeetingTypeByIdAsync(int meetingTypeId);
        #endregion MeetingTypes

        #region MeetingNotes
        public Task AddNoteAsync(Guid id, string content, bool feedbackRequired);
        public Task UpdateNoteAsync(Guid id, string content, bool feedbackRequired);
        public Task DeleteNoteAsync(Guid id);
        public Task<List<MeetingNote>> GetMeetingNotesAsync(Guid id);
        public Task<List<MeetingNote>> GetMeetingFeedbackRequiredNotesAsync(Guid id);
        #endregion MeetingNotes

        #region MeetingGoals
        public Task AddGoalAsync(Guid id, string content);
        public Task UpdateGoalTaskAsync(Guid id, string content);
        public Task DeleteGoalAsync(Guid id);
        public Task<List<MeetingGoal>> GetMeetingGoalsAsync(Guid id);
        public Task<List<MeetingGoal>> GetMeetingGoalsByPersonAsync(long personId);
        public Task CompleteGoalAsync(Guid goalId, string completeDescription);
        #endregion MeetingGoals
    }
}

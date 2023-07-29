using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IMeetingService
    {
        public Task<List<Meeting>> GetMeetingsAsync(long? personId);

        public Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId);
        public Task<List<Meeting>> GetMeetingsByPersonIdAsync(long personId);
        public Task CreateMeetingTypeAsync(MeetingType mt);
        public Task UpdateMeetingTypeAsync(MeetingType mt);
        public Task DeleteMeetingTypeAsync(int id);
        public Task<List<MeetingType>> GetMeetingTypesAsync();
        public Task<Meeting> CreateMeetingAsync(Meeting m);
        public Task UpdateMeetingAsync(MeetingDTO mtdto);
        public Task DeleteMeetingAsync(Guid id);
        public Task AddNoteAsync(Guid id, string content, bool feedbackRequired);
        public Task UpdateNoteAsync(Guid id, string content, bool feedbackRequired);
        public Task DeleteNoteAsync(Guid id);
        public Task<List<MeetingNote>> GetMeetingNotesAsync(Guid id);
        public Task<List<MeetingNote>> GetMeetingFeedbackRequiredNotesAsync(Guid id);
        public Task AddGoalAsync(Guid id, string content);
        public Task UpdateGoalTaskAsync(Guid id, string content);
        public Task DeleteGoalAsync(Guid id);
        public Task<List<MeetingGoal>> GetMeetingGoalsAsync(Guid id);
        public Task<Meeting?> GetLastOneToOneByPersonIdAsync(long personId);
        public Task MarkAsSendedFollowUpAsync(Guid meetingId);
        public Task<List<MeetingGoal>> GetMeetingGoalsByPersonAsync(long personId);
        public Task<DateTime?> GetFactMeetingDateByIdAsync(Guid meetingId);
        public Task CompleteGoalAsync(Guid goalId, string completeDescription);
    }
}

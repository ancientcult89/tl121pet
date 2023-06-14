using tl121pet.Entities.DTO;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneService
    {
        public Task<List<OneToOneDeadline>> GetDeadLinesAsync();
        public Task<string> GenerateFollowUpAsync(Guid meetingId, long personId);
        public Task SendFollowUpAsync(Guid meetingId, long personId);
        public Task<string> GetMeetingNoteAndGoalsAsync(Guid meetingId);
        public Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId);
    }
}

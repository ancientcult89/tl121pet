using tl121pet.Entities.DTO;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneService
    {
        public Task<List<OneToOneDeadline>> GetDeadLinesAsync();
        public string GenerateFollowUp(Guid meetingId, long personId);
        public void SendFollowUp(Guid meetingId, long personId);
        public string GetMeetingNoteAndGoals(Guid meetingId);
        public string GetPreviousMeetingNoteAndGoals(Guid meetingId, long personId);
    }
}

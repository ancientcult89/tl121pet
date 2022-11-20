using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneService
    {
        public List<OneToOneDeadline> GetDeadLines();
        public string GenerateFollowUp(Guid meetingId, long personId);
        public void SendFollowUp(Guid meetingId, long personId);
        public string GetMeetingNoteAndGoals(Guid meetingId);
    }
}

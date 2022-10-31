using tl121pet.Entities.DTO;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneService
    {
        public List<OneToOneDeadline> GetDeadLines();
        public string GenerateFollowUp(Guid meetingId, long personId);
    }
}

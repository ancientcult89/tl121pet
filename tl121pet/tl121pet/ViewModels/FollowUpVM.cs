using tl121pet.Storage;

namespace tl121pet.ViewModels
{
    public class FollowUpVM
    {
        public string FollowUpMessage { get; set; } = string.Empty;
        public Guid MeetingId { get; set; }
        public FormMode Mode { get; set; }
        public long PersonId { get; set; }
    }
}

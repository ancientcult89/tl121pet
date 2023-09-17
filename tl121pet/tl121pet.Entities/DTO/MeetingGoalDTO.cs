namespace tl121pet.Entities.DTO
{
    public class MeetingGoalDTO
    {
        public Guid? MeetingGoalId { get; set; }
        public string MeetingGoalDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public Guid MeetingId { get; set; }
    }
}

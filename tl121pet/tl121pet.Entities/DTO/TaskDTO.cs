namespace tl121pet.Entities.DTO
{
    public class TaskDTO
    {
        public Guid MeetingGoalId { get; set; }
        public string MeetingGoalDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public string CompleteDescription { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public long PersonId { get; set; }
    }
}

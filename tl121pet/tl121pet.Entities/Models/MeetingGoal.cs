﻿namespace tl121pet.Entities.Models
{
    public class MeetingGoal
    {
        public Guid MeetingGoalId { get; set; }
        public string MeetingGoalDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public string CompleteDescription { get; set; } = string.Empty;
        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = new Meeting();
    }
}

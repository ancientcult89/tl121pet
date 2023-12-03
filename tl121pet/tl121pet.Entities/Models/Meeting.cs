//встречи 1-2-1.

namespace tl121pet.Entities.Models
{
    public class Meeting
    {
        public Guid MeetingId { get; set; }
        public DateTime MeetingPlanDate { get; set; } = DateTime.Now;
        public DateTime? MeetingDate { get; set; }

        public bool FollowUpIsSended { get; set; } = false;

        public long PersonId { get; set; }
        public Person Person { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

        public List<MeetingGoal> MeetingGoals { get; set; }
        public List<MeetingNote> MeetingNotes { get; set; }
    }
}

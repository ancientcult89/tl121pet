using System.ComponentModel.DataAnnotations;

namespace tl121pet.Entities.DTO
{
    public class MeetingDTO
    {
        public Guid MeetingId { get; set; }
        public DateTime MeetingPlanDate { get; set; } = DateTime.Now;
        public DateTime? MeetingDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="Required")]
        public int MeetingTypeId { get; set; }

        public bool FollowUpIsSended { get; set; } = false;

        [Range(1, long.MaxValue, ErrorMessage = "Required")]
        public long PersonId { get; set; }
    }
}

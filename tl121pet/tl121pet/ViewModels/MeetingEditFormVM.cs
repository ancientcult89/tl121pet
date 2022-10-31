using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class MeetingEditFormVM : SimpleEditFormVM<Meeting>
    {
        public List<MeetingNote>? MeetingNotes { get; set; }
        public string? NewNote { get; set; } = "";

        public List<MeetingGoal>? MeetingGoals { get; set; }
        public string? NewGoal { get; set; } = "";
    }
}

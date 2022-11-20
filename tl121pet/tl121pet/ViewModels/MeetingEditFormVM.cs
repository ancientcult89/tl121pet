using tl121pet.Entities.Models;

namespace tl121pet.ViewModels
{
    public class MeetingEditFormVM : SimpleEditFormVM<Meeting>
    {
        public List<MeetingNote>? MeetingNotes { get; set; }
        public string? NewNote { get; set; } = "";
        public bool NewNoteFeedbackRequires { get; set; } = false;

        public List<MeetingGoal>? MeetingGoals { get; set; }
        public string? NewGoal { get; set; } = "";
        public string? PreviousMeetingNotesAndGoals { get; set; } = "";
    }
}

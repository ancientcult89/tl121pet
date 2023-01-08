namespace tl121pet.ViewModels
{
    //selectedItem - meetingId
    public class NoteEditListVM : SimpleEditFormVM<Guid>
    {
        public string NewNote { get; set; } = String.Empty;
        public bool NewNoteFeedbackRequires { get; set; } = false;
    }
}

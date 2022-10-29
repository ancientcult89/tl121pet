using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IMeetingRepository
    {
        public void CreateMeetingType(MeetingType mt);
        public void UpdateMeetingType(MeetingType mt);
        public void DeleteMeetingType(int id);
        public List<MeetingType> GetMeetingTypes();
        public void CreateMeeting(Meeting m);
        public void UpdateMeeting(Meeting mt);
        public void DeleteMeeting(Guid id);
        public void AddNote(Guid id, string content);
        public void DeleteNote(Guid id);
        public List<MeetingNote> GetMeetingNotes(Guid id);
        public void AddGoal(Guid id, string content);
        public void DeleteGoal(Guid id);
        public List<MeetingGoal> GetMeetingGoals(Guid id);

    }
}

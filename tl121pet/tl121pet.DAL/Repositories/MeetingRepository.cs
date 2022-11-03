using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private DataContext _dataContext;
        public MeetingRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        #region MeetingType
        public void CreateMeetingType(MeetingType mt)
        {
            _dataContext.MeetingTypes.Add(mt);
            _dataContext.SaveChanges();
        }

        public void DeleteMeetingType(int id)
        {
            var meetingTypeToDelete = _dataContext.MeetingTypes.Find(id);
            _dataContext.MeetingTypes.Remove(meetingTypeToDelete);
            _dataContext.SaveChanges();
        }

        public void UpdateMeetingType(MeetingType mt)
        {
            _dataContext.MeetingTypes.Update(mt);
            _dataContext.SaveChanges();
        }

        public List<MeetingType> GetMeetingTypes()
        { 
            return _dataContext.MeetingTypes.ToList();
        }
        #endregion MeetingType

        #region Meeting
        public void CreateMeeting(Meeting m)
        {
            m.MeetingGoals = default;
            m.MeetingNotes = default;
            _dataContext.Meetings.Add(m);
            _dataContext.SaveChanges();
        }

        public void UpdateMeeting(Meeting m)
        {
            _dataContext.Meetings.Update(m);
            _dataContext.SaveChanges();
        }

        public void DeleteMeeting(Guid id)
        {
            var meetingTypeToDelete = _dataContext.Meetings.Find(id);
            _dataContext.Meetings.Remove(meetingTypeToDelete);
            _dataContext.SaveChanges();
        }

        public void AddNote(Guid id, string content, bool feedbackRequired)
        {
            _dataContext.MeetingNotes.Add(new MeetingNote { MeetingId = id, Meeting = default, MeetingNoteContent = content, FeedbackRequired = feedbackRequired });
            _dataContext.SaveChanges();
        }

        public void DeleteNote(Guid id)
        {
            var meetingNoteToDelete = _dataContext.MeetingNotes.Find(id);
            _dataContext.MeetingNotes.Remove(meetingNoteToDelete);
            _dataContext.SaveChanges();
        }

        public List<MeetingNote> GetMeetingNotes(Guid id)
        { 
            return _dataContext.MeetingNotes.Where(p => p.MeetingId == id).ToList();
        }

        public List<MeetingNote> GetMeetingFeedbackRequiredNotes(Guid id)
        {
            return _dataContext.MeetingNotes.Where(p => p.MeetingId == id && p.FeedbackRequired == true).ToList();
        }

        
        public void AddGoal(Guid id, string content)
        {
            _dataContext.MeetingGoals.Add(new MeetingGoal { MeetingId = id, Meeting = default, MeetingGoalDescription = content });
            _dataContext.SaveChanges();
        }

        public void DeleteGoal(Guid id)
        {
            var meetingGoalToDelete = _dataContext.MeetingGoals.Find(id);
            _dataContext.MeetingGoals.Remove(meetingGoalToDelete);
            _dataContext.SaveChanges();
        }

        public List<MeetingGoal> GetMeetingGoals(Guid id)
        {
            return _dataContext.MeetingGoals.Where(p => p.MeetingId == id).ToList();
        }
        #endregion Meeting

        public Meeting? GetLastOneToOneByPersonId(long personId) 
        { 
            return _dataContext.Meetings
                .Where(p => p.PersonId == personId && p.MeetingDate != null)
                .OrderByDescending(p => p.MeetingDate)
                .Take(1).FirstOrDefault() ;
        }

        public void MarAsSendedFollowUp(Guid meetingId)
        {
            Meeting meeting = _dataContext.Meetings.Find(meetingId);
            if (meeting != null)
            { 
                meeting.FollowUpIsSended = true;
                _dataContext.Update(meeting);
                _dataContext.SaveChanges();
            }
        }
    }
}

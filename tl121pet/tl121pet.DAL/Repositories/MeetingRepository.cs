using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
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
        public async Task CreateMeetingTypeAsync(MeetingType mt)
        {
            _dataContext.MeetingTypes.Add(mt);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteMeetingTypeAsync(int id)
        {
            var meetingTypeToDelete = _dataContext.MeetingTypes.Find(id);
            _dataContext.MeetingTypes.Remove(meetingTypeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateMeetingTypeAsync(MeetingType mt)
        {
            _dataContext.MeetingTypes.Update(mt);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<MeetingType>> GetMeetingTypesAsync()
        { 
            return await _dataContext.MeetingTypes.ToListAsync();
        }
        #endregion MeetingType

        #region Meeting
        public async Task<Meeting> CreateMeetingAsync(Meeting m)
        {
            m.MeetingGoals = default;
            m.MeetingNotes = default;
            _dataContext.Meetings.Add(m);
            await _dataContext.SaveChangesAsync();
            return m;
        }

        public async Task UpdateMeetingAsync(MeetingDTO meetingDTO)
        {
            Meeting editedMeeting = await _dataContext.Meetings.FindAsync(meetingDTO.MeetingId);
            editedMeeting.MeetingPlanDate = meetingDTO.MeetingPlanDate;
            editedMeeting.MeetingDate = meetingDTO.MeetingDate;
            editedMeeting.MeetingTypeId = meetingDTO.MeetingTypeId;
            editedMeeting.FollowUpIsSended = meetingDTO.FollowUpIsSended;
            editedMeeting.PersonId = meetingDTO.PersonId;
            _dataContext.Meetings.Update(editedMeeting);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteMeetingAsync(Guid id)
        {
            var meetingTypeToDelete = await _dataContext.Meetings.FindAsync(id);
            _dataContext.Meetings.Remove(meetingTypeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task AddNoteAsync(Guid id, string content, bool feedbackRequired)
        {
            _dataContext.MeetingNotes.Add(new MeetingNote { MeetingId = id, Meeting = default, MeetingNoteContent = content, FeedbackRequired = feedbackRequired });
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(Guid id)
        {
            var meetingNoteToDelete = await _dataContext.MeetingNotes.FindAsync(id);
            _dataContext.MeetingNotes.Remove(meetingNoteToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<MeetingNote>> GetMeetingNotesAsync(Guid id)
        { 
            return await _dataContext.MeetingNotes.Where(p => p.MeetingId == id).ToListAsync();
        }

        public async Task<List<MeetingNote>> GetMeetingFeedbackRequiredNotesAsync(Guid id)
        {
            return await _dataContext.MeetingNotes
                .Where(p => p.MeetingId == id && p.FeedbackRequired == true)
                .ToListAsync();
        }

        
        public async Task AddGoalAsync(Guid id, string content)
        {
            _dataContext.MeetingGoals.Add(new MeetingGoal { MeetingId = id, Meeting = default, MeetingGoalDescription = content });
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteGoalAsync(Guid id)
        {
            var meetingGoalToDelete = await _dataContext.MeetingGoals.FindAsync(id);
            _dataContext.MeetingGoals.Remove(meetingGoalToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<MeetingGoal>> GetMeetingGoalsAsync(Guid id)
        {
            return await _dataContext.MeetingGoals
                .Where(p => p.MeetingId == id)
                .ToListAsync();
        }
        #endregion Meeting

        public async Task<Meeting?> GetLastOneToOneByPersonIdAsync(long personId) 
        { 
            return await _dataContext.Meetings
                .Where(p => p.PersonId == personId && p.MeetingDate != null)
                .OrderByDescending(p => p.MeetingDate)
                .Take(1).FirstOrDefaultAsync() ;
        }

        public async Task MarkAsSendedFollowUpAsync(Guid meetingId)
        {
            Meeting meeting = await _dataContext.Meetings.FindAsync(meetingId);
            if (meeting != null)
            { 
                meeting.FollowUpIsSended = true;
                _dataContext.Update(meeting);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task UpdateNoteAsync(Guid id, string MeetingNoteContent, bool feedbackRequired)
        {
            MeetingNote mn = await _dataContext.MeetingNotes.FindAsync(id);
            mn.MeetingNoteContent = MeetingNoteContent;
            mn.FeedbackRequired = feedbackRequired;
            _dataContext.MeetingNotes.Update(mn);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateGoalTaskAsync(Guid id, string content)
        {
            MeetingGoal mg = await _dataContext.MeetingGoals.FindAsync(id);
            mg.MeetingGoalDescription = content;
            _dataContext.MeetingGoals.Update(mg);
            _dataContext.SaveChangesAsync();
        }

        public async Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId)
        {
            Meeting previousMeeting = await _dataContext.Meetings
                .OrderByDescending(p => p.MeetingDate)
                .Where(p => p.PersonId == personId && p.MeetingId != currnetMeetingId)
                .FirstOrDefaultAsync();
            return previousMeeting?.MeetingId;
        }

        public async Task<List<Meeting>> GetMeetingsByPersonIdAsync(long personId)
        {
            return await _dataContext.Meetings
                .Include(mt => mt.MeetingType)
                .Include(mt => mt.Person)
                .Where(mt => mt.PersonId == personId)
                .ToListAsync();
        }

        public async Task<List<MeetingGoal>> GetMeetingGoalsByPersonAsync(long personId)
        {
            //TODO: переделать на асинхронный LINQ
            List<MeetingGoal> meetingGoals = new List<MeetingGoal>();

            var searchedGoals = from goals in _dataContext.MeetingGoals
                           join meeting in _dataContext.Meetings on goals.MeetingId equals meeting.MeetingId
                           join peop in _dataContext.People on meeting.PersonId equals peop.PersonId
                           where peop.PersonId == personId
                           select goals;

            foreach (MeetingGoal goal in searchedGoals)
            { 
                meetingGoals.Add(goal);
            }

            return meetingGoals;
        }

        public async Task<DateTime?> GetFactMeetingDateByIdAsync(Guid meetingId)
        {
            return await _dataContext.Meetings
                .Where(m => m.MeetingId == meetingId)
                .Select(m => m.MeetingDate)
                .FirstOrDefaultAsync();
        }

        public async Task CompleteGoalAsync(Guid goalId, string completeDescription)
        {
            MeetingGoal goal = await _dataContext.MeetingGoals.Where(g => g.MeetingGoalId == goalId).FirstOrDefaultAsync();
            goal.CompleteDescription = completeDescription;
            goal.IsCompleted = true;
            _dataContext.MeetingGoals.Update(goal);
            await _dataContext.SaveChangesAsync();
        }
    }
}

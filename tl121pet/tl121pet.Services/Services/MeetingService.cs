using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class MeetingService : IMeetingService
    {
        private DataContext _dataContext;

        public MeetingService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #region Meeting
        public async Task<Meeting> CreateMeetingAsync(Meeting newMeeting)
        {
            await CheckDuplicatedMeetingAsync(newMeeting);

            newMeeting.MeetingGoals = default;
            newMeeting.MeetingNotes = default;
            _dataContext.Meetings.Add(newMeeting);
            await _dataContext.SaveChangesAsync();
            return newMeeting;
        }

        public async Task<List<Meeting>> GetMeetingsByPersonAsync(List<Person> people)
        {
            List<Meeting> meetingsByPerson = new List<Meeting>();
            foreach (Person person in people)
            {
                meetingsByPerson.AddRange(await GetMeetingsByPersonIdAsync(person.PersonId));
            }
            return meetingsByPerson;
        }
        
        public async Task<Meeting> GetMeetingByIdAsync(Guid id)
        {
            return await _dataContext.Meetings.FindAsync(id) ?? throw new Exception("Meeting not found");
        }

        public async Task<Meeting> UpdateMeetingAsync(Meeting editedMeeting)
        {
            Meeting modifiedMeeting = await GetMeetingByIdAsync(editedMeeting.MeetingId);
            await CheckDuplicatedMeetingAsync(editedMeeting);

            _dataContext.Entry(modifiedMeeting).CurrentValues.SetValues(editedMeeting);
            await _dataContext.SaveChangesAsync();
            return editedMeeting;
        }

        public async Task DeleteMeetingAsync(Guid id)
        {
            Meeting meetingTypeToDelete = await GetMeetingByIdAsync(id);

            _dataContext.Meetings.Remove(meetingTypeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        #endregion Meeting

        #region Note
        [Obsolete]
        public async Task<MeetingNote> AddNoteAsync(Guid id, string content, bool feedbackRequired)
        {
            MeetingNote newNote = new MeetingNote { MeetingId = id, Meeting = default, MeetingNoteContent = content, FeedbackRequired = feedbackRequired };
            _dataContext.MeetingNotes.Add(newNote);
            await _dataContext.SaveChangesAsync();
            return newNote;
        }

        public async Task<MeetingNote> AddNoteAsync(MeetingNote newNote)
        {
            Meeting processingMeeting = await GetMeetingByIdAsync(newNote.MeetingId);

            newNote.Meeting = processingMeeting;
            _dataContext.MeetingNotes.Add(newNote);
            await _dataContext.SaveChangesAsync();
            return newNote;
        }

        [Obsolete]
        public async Task UpdateNoteAsync(Guid id, string MeetingNoteContent, bool feedbackRequired)
        {
            MeetingNote mn = await _dataContext.MeetingNotes.FindAsync(id);
            mn.MeetingNoteContent = MeetingNoteContent;
            mn.FeedbackRequired = feedbackRequired;
            _dataContext.MeetingNotes.Update(mn);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<MeetingNote> UpdateNoteAsync(MeetingNote meetingNote)
        {
            Meeting processingMeeting = await GetMeetingByIdAsync(meetingNote.MeetingId);
            meetingNote.Meeting = processingMeeting;
            MeetingNote updatedNote = await GetMeetingNoteByIdAsync(meetingNote.MeetingNoteId);

            _dataContext.Entry(updatedNote).CurrentValues.SetValues(meetingNote);
            await _dataContext.SaveChangesAsync();
            return meetingNote;
        }

        public async Task DeleteNoteAsync(Guid id)
        {
            MeetingNote meetingNoteToDelete = await GetMeetingNoteByIdAsync(id);
            _dataContext.MeetingNotes.Remove(meetingNoteToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<MeetingNote>> GetMeetingNotesAsync(Guid meetingId)
        {
            return await _dataContext.MeetingNotes.Where(p => p.MeetingId == meetingId).ToListAsync();
        }

        public async Task<List<MeetingNote>> GetMeetingFeedbackRequiredNotesAsync(Guid id)
        {
            return await _dataContext.MeetingNotes
                .Where(p => p.MeetingId == id && p.FeedbackRequired == true)
                .ToListAsync();
        }

        #endregion Note

        #region Goal
        [Obsolete]
        public async Task AddGoalAsync(Guid id, string content)
        {
            _dataContext.MeetingGoals.Add(new MeetingGoal { MeetingId = id, Meeting = default, MeetingGoalDescription = content });
            await _dataContext.SaveChangesAsync();
        }

        public async Task<MeetingGoal> AddGoalAsync(MeetingGoal newGoal)
        {
            Meeting meeting = await GetMeetingByIdAsync(newGoal.MeetingId);
            newGoal.Meeting = meeting;
            _dataContext.MeetingGoals.Add(newGoal);
            await _dataContext.SaveChangesAsync();
            return newGoal;
        }

        [Obsolete]
        public async Task UpdateGoalTaskAsync(Guid id, string content)
        {
            MeetingGoal mg = await _dataContext.MeetingGoals.FindAsync(id);
            mg.MeetingGoalDescription = content;
            _dataContext.MeetingGoals.Update(mg);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<MeetingGoal> UpdateGoalAsync(MeetingGoal meetingGoal)
        {
            Meeting processingMetting = await GetMeetingByIdAsync(meetingGoal.MeetingId);
            MeetingGoal modifiedGoal = await GetMeetingGoalByIdAsync(meetingGoal.MeetingGoalId);
            meetingGoal.Meeting = processingMetting;
            _dataContext.Entry(modifiedGoal).CurrentValues.SetValues(meetingGoal);
            await _dataContext.SaveChangesAsync();
            return meetingGoal;
        }

        public async Task DeleteGoalAsync(Guid goalId)
        {
            MeetingGoal meetingGoalToDelete = await GetMeetingGoalByIdAsync(goalId);
            _dataContext.MeetingGoals.Remove(meetingGoalToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<MeetingGoal>> GetMeetingGoalsAsync(Guid id)
        {
            return await _dataContext.MeetingGoals
                .Where(p => p.MeetingId == id)
                .ToListAsync();
        }

        #endregion Goal

        #region MeetingProcessing
        public async Task<Meeting?> GetLastOneToOneByPersonIdAsync(long personId)
        {
            return await _dataContext.Meetings
                .Where(p => p.PersonId == personId && p.MeetingDate != null)
                .OrderByDescending(p => p.MeetingDate)
                .Take(1).FirstOrDefaultAsync();
        }

        public async Task MarkAsSendedFollowUpAndFillActualDateAsync(Guid meetingId)
        {
            Meeting meeting = await _dataContext.Meetings.FindAsync(meetingId);
            if (meeting != null)
            {
                meeting.FollowUpIsSended = true;
                meeting.MeetingDate = DateTime.Now;
                _dataContext.Update(meeting);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId)
        {
            Meeting previousMeeting = await _dataContext.Meetings
                .OrderByDescending(p => p.MeetingDate)
                .Where(p => p.PersonId == personId && p.MeetingId != currnetMeetingId)
                .FirstOrDefaultAsync();
            return previousMeeting?.MeetingId;
        }

        public async Task<List<MeetingGoal>> GetMeetingGoalsByPersonAsync(long personId)
        {
            List<MeetingGoal> meetingGoals = new List<MeetingGoal>();

            var searchedGoals = (
                from goals in _dataContext.MeetingGoals
                join meeting in _dataContext.Meetings on goals.MeetingId equals meeting.MeetingId
                join peop in _dataContext.People on meeting.PersonId equals peop.PersonId
                where peop.PersonId == personId
                select goals
            ).ToListAsync();

            foreach (MeetingGoal goal in await searchedGoals)
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

        public async Task CompleteGoalAsync(Guid goalId)
        {
            MeetingGoal goal = await _dataContext.MeetingGoals.Where(g => g.MeetingGoalId == goalId).FirstOrDefaultAsync();
            goal.IsCompleted = true;
            _dataContext.MeetingGoals.Update(goal);
            await _dataContext.SaveChangesAsync();
        }
        #endregion MeetingProcessing

        private async Task<List<Meeting>> GetMeetingsByPersonIdAsync(long personId)
        {
            return await _dataContext.Meetings
                .Include(mt => mt.Person)
                .Where(mt => mt.PersonId == personId)
                .ToListAsync();
        }

        private async Task CheckDuplicatedMeetingAsync(Meeting newMeeting)
        {
            var existsMeeting = _dataContext.Meetings
                .Where(m => m.PersonId == newMeeting.PersonId && m.MeetingPlanDate == newMeeting.MeetingPlanDate && m.MeetingId != newMeeting.MeetingId)
                .FirstOrDefault();
            if (existsMeeting != null)
                throw new Exception("The Meeting with this person on that date is already planned");
        }

        private async Task<MeetingNote> GetMeetingNoteByIdAsync(Guid noteId)
        {
            return _dataContext.MeetingNotes.Find(noteId) ?? throw new Exception("Note not found");
        }

        private async Task<MeetingGoal> GetMeetingGoalByIdAsync(Guid goalId)
        {
            return _dataContext.MeetingGoals.Find(goalId) ?? throw new Exception("Goal not found");
        }
    }
}

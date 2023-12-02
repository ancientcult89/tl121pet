using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure.Exceptions;
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

        public async Task<Meeting> CreateCurrentMeetingByPersonIdAsync(long userId, long personId)
        {
            Meeting newMeeting = new Meeting() { 
                PersonId = personId,
                UserId = userId,
                MeetingPlanDate = DateTime.Now.Date,
            };
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
        
        public async Task<Meeting> GetMeetingByIdMeetingAsync(Guid id)
        {
            return await _dataContext.Meetings.FindAsync(id) ?? throw new DataFoundException("Meeting not found");
        }

        public async Task<List<Meeting>> GetMeetingsByUserIdAsync(long userId, long? personId)
        {
            return await _dataContext.Meetings
                .Include(mt => mt.Person)
                .Where(m => m.UserId == userId && (personId == null || m.PersonId == personId))
                .ToListAsync();
        }

        public async Task<Meeting> UpdateMeetingAsync(Meeting editedMeeting)
        {
            Meeting modifiedMeeting = await GetMeetingByIdMeetingAsync(editedMeeting.MeetingId);
            await CheckDuplicatedMeetingAsync(editedMeeting);

            _dataContext.Entry(modifiedMeeting).CurrentValues.SetValues(editedMeeting);
            await _dataContext.SaveChangesAsync();
            return editedMeeting;
        }

        public async Task DeleteMeetingAsync(Guid id)
        {
            Meeting meetingTypeToDelete = await GetMeetingByIdMeetingAsync(id);

            _dataContext.Meetings.Remove(meetingTypeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        #endregion Meeting

        #region Note
        public async Task<MeetingNote> AddNoteAsync(MeetingNote newNote)
        {
            Meeting processingMeeting = await GetMeetingByIdMeetingAsync(newNote.MeetingId);

            _dataContext.MeetingNotes.Add(newNote);
            await _dataContext.SaveChangesAsync();
            return newNote;
        }

        public async Task<MeetingNote> UpdateNoteAsync(MeetingNote meetingNote)
        {
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
        public async Task<MeetingGoal> AddGoalAsync(MeetingGoal newGoal)
        {
            Meeting meeting = await GetMeetingByIdMeetingAsync(newGoal.MeetingId);

            _dataContext.MeetingGoals.Add(newGoal);
            await _dataContext.SaveChangesAsync();
            return newGoal;
        }

        public async Task<MeetingGoal> UpdateGoalAsync(MeetingGoal meetingGoal)
        {
            MeetingGoal modifiedGoal = await GetMeetingGoalByIdAsync(meetingGoal.MeetingGoalId);
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
        public async Task CompleteGoalAsync(Guid goalId)
        {
            MeetingGoal goal = await GetMeetingGoalByIdAsync(goalId);
            goal.IsCompleted = true;
            _dataContext.MeetingGoals.Update(goal);
            await _dataContext.SaveChangesAsync();
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

        public async Task<Meeting> MarkAsSendedFollowUpAndFillActualDateAsync(Guid meetingId, DateTime actualDate)
        {
            Meeting meeting = await GetMeetingByIdMeetingAsync(meetingId);
            if (meeting != null)
            {
                meeting.FollowUpIsSended = true;
                meeting.MeetingDate = actualDate;
                _dataContext.Update(meeting);
                await _dataContext.SaveChangesAsync();
            }
            return meeting;
        }

        public async Task<Guid?> GetPreviousMeetingIdAsync(Guid currnetMeetingId, long personId)
        {
            Meeting previousMeeting = await _dataContext.Meetings
                .OrderByDescending(p => p.MeetingDate)
                .Where(p => p.PersonId == personId && p.MeetingId != currnetMeetingId)
                .FirstOrDefaultAsync();
            return previousMeeting?.MeetingId;
        }

        [Obsolete]
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
        #endregion MeetingProcessing

        #region Tasks
        public async Task<List<TaskDTO>> GetTasksByUserId(long userId)
        {
            List<TaskDTO> tasks = await (
                from g in _dataContext.MeetingGoals
                join m in _dataContext.Meetings on g.MeetingId equals m.MeetingId
                join p in _dataContext.People on m.PersonId equals p.PersonId
                where m.UserId == userId
                select new TaskDTO {
                    MeetingGoalId = g.MeetingGoalId,
                    IsCompleted = g.IsCompleted,
                    MeetingGoalDescription = g.MeetingGoalDescription,
                    PersonName = p.LastName + " " + p.FirstName + " " + p.SurName,
                    PersonId = p.PersonId,
                    FactDate = m.MeetingDate
                }
            ).ToListAsync();

            return tasks;
        }
        #endregion

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
                throw new LogicException("The Meeting with this person on that date is already planned");
        }

        private async Task<MeetingNote> GetMeetingNoteByIdAsync(Guid noteId)
        {
            return _dataContext.MeetingNotes.Find(noteId) ?? throw new DataFoundException("Note not found");
        }

        private async Task<MeetingGoal> GetMeetingGoalByIdAsync(Guid goalId)
        {
            return _dataContext.MeetingGoals.Find(goalId) ?? throw new DataFoundException("Goal not found");
        }
    }
}

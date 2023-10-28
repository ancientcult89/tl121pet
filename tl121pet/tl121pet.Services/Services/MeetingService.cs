using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class MeetingService : IMeetingService
    {

        private readonly IAuthService _authService;
        private readonly IPersonService _personService;
        private DataContext _dataContext;

        public MeetingService(IAuthService authService
            , IPersonService personService
            , DataContext dataContext)
        {
            _authService = authService;
            _personService = personService;
            _dataContext = dataContext;
        }

        public async Task<List<Meeting>> GetMeetingsAsync(long? personId)
        {
            List<Meeting> meetingsRes = new List<Meeting>();
            long? userId = _authService.GetMyUserId();
            if (userId != null)
            {
                List<Person> people = new List<Person>();
                List<ProjectTeam> projects = new List<ProjectTeam>();
                projects = await _authService.GetUserProjectsAsync((long)userId);
                people = await GetPeopleByProjectsAsync(projects, personId);
                meetingsRes = await GetMeetingsByPersonAsync(people);
            }

            return meetingsRes
                .OrderByDescending(m => m.Person.LastName)
                .OrderByDescending(m => m.MeetingPlanDate)
                .OrderByDescending(m => m.MeetingDate)
                .ToList();
        }

        //TODO: странное месторасположение, лучше перенести в PersonService
        private async Task<List<Person>> GetPeopleByProjectsAsync(List<ProjectTeam> projects, long? personId)
        {
            List<Person> personByProjects = new List<Person>();

            foreach (ProjectTeam pt in projects)
            {
                personByProjects.AddRange(await _personService.GetPeopleFilteredByProjectAsync(pt.ProjectTeamId));
            }

            if (personId != null)
                personByProjects = personByProjects.Where(p => p.PersonId == (long)personId).ToList();

            return personByProjects.Distinct(new PersonComparer()).ToList();
        }

        private async Task<List<Meeting>> GetMeetingsByPersonAsync(List<Person> people)
        {
            List<Meeting> meetingsByPerson = new List<Meeting>();
            foreach (Person person in people)
            {
                meetingsByPerson.AddRange(await GetMeetingsByPersonIdAsync(person.PersonId));
            }
            return meetingsByPerson;
        }

        #region Meeting
        public async Task<Meeting> GetMeetingByIdAsync(Guid id)
        {
            return await _dataContext.Meetings.FindAsync(id) ?? throw new Exception("Meeting not found");
        }
        public async Task<Meeting> CreateMeetingAsync(Meeting m)
        {
            m.MeetingGoals = default;
            m.MeetingNotes = default;
            _dataContext.Meetings.Add(m);
            await _dataContext.SaveChangesAsync();
            return m;
        }

        public async Task<Meeting> UpdateMeetingAsync(Meeting editedMeeting)
        {
            _dataContext.Meetings.Update(editedMeeting);
            await _dataContext.SaveChangesAsync();
            return editedMeeting;
        }

        public async Task DeleteMeetingAsync(Guid id)
        {
            var meetingTypeToDelete = await _dataContext.Meetings.FindAsync(id);
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
            newNote.Meeting = default;
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
            meetingNote.Meeting = default;
            _dataContext.MeetingNotes.Update(meetingNote);
            await _dataContext.SaveChangesAsync();
            return meetingNote;
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
            newGoal.Meeting = default;
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
            meetingGoal.Meeting = default;
            _dataContext.MeetingGoals.Update(meetingGoal);
            await _dataContext.SaveChangesAsync();
            return meetingGoal;
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

        public async Task<List<Meeting>> GetMeetingsByPersonIdAsync(long personId)
        {
            return await _dataContext.Meetings
                .Include(mt => mt.Person)
                .Where(mt => mt.PersonId == personId)
                .ToListAsync();
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
    }
}

using tl121pet.Entities.Aggregate;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{

    public class OneToOneApplication : IOneToOneApplication
    {
        private IPersonService _personService;
        private IMeetingService _meetingService;
        private IMailService _mailService;
        private readonly IAuthService _authService;

        public OneToOneApplication(
            IMeetingService meetingService,
            IMailService mailService,
            IPersonService personService,
            IAuthService authService)
        {
            _meetingService = meetingService;
            _mailService = mailService;
            _personService = personService;
            _authService = authService;
        }

        public async Task<List<OneToOneDeadline>> GetDeadLinesAsync()
        {
            List<OneToOneDeadline> deadLines = new List<OneToOneDeadline>();
            
            foreach (Person p in await GetPeopleFilteredByProjectsAsync())
            {
                TimeSpan datediff = new TimeSpan();
                Meeting lastMeeting = await _meetingService.GetLastOneToOneByPersonIdAsync(p.PersonId) ?? new Meeting();
                if (lastMeeting.MeetingDate != null)
                {
                    DateTime lastMeetingDate = (DateTime)lastMeeting.MeetingDate;
                    datediff = lastMeetingDate.AddMonths(1).Date - DateTime.Now.Date;
                }

                DateTime deadlineDate = lastMeeting.MeetingDate != null ? ((DateTime)lastMeeting.MeetingDate).AddMonths(1) : DateTime.Now;

                deadLines.Add(new OneToOneDeadline
                {
                    Person = p,
                    LastMeetingOneToOne = lastMeeting,
                    LastOneToOneMeetingDate = lastMeeting.MeetingDate,
                    DeadLine = deadlineDate,
                    DayToDeadline = datediff.Days,
                });
            }
            return deadLines;
        }

        public async Task<string> GenerateFollowUpAsync(Guid meetingId, long personId)
        {
            string result = "";
            Person person = await _personService.GetPersonByIdAsync(personId);
            result = $"{(!String.IsNullOrEmpty(person.ShortName) ? person.ShortName : person.FirstName)}, спасибо за проведённую встречу!\n\n";
            result += await GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(meetingId);
            result += "\n\nЕсли что-то упустил - обязательно сообщи мне об этом!";
            return result;
        }

        public async Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId)
        {
            string prevNoteAndGoals = "";
            Guid? previousMeetingGuid = await _meetingService.GetPreviousMeetingIdAsync(meetingId, personId);
            if (previousMeetingGuid != null)
            {
                prevNoteAndGoals = await GetMeetingFeedbackRequiredNotesAndGoalByMeetingId((Guid)previousMeetingGuid);
            }
            return prevNoteAndGoals;
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(Guid meetingId)
        {
            string result = "";
            result += await GetMeetingFeedbackRequiredNotesByMeetingId(meetingId);
            result += "\n\n";
            result += await GetMeetingGoalsByMeetingId(meetingId);

            return result;
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesByMeetingId(Guid meetingId)
        {
            string meetingGoals = "";
            List<MeetingNote> notes = await _meetingService.GetMeetingFeedbackRequiredNotesAsync(meetingId);
            if (notes.Count() > 0)
            {
                meetingGoals += "На встрече обсудили:\n";
                foreach (MeetingNote mn in notes)
                {
                    meetingGoals += $"\t- {mn.MeetingNoteContent};\n";
                }
            }

            return meetingGoals;
        }

        private async Task<string> GetMeetingGoalsByMeetingId(Guid meetingId)
        {
            string meetingGoals = "";
            List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsAsync(meetingId);
            if (goals.Count() > 0)
            {
                meetingGoals += "К следующему 1-2-1 договорились:\n";
                foreach (MeetingGoal mg in goals)
                {
                    meetingGoals += $"\t- {mg.MeetingGoalDescription};\n";
                }
            }

            return meetingGoals;
        }

        //TODO: на вход должнен подаваться айдишка встречи и уже готовая почта
        public async Task SendFollowUpAsync(Guid meetingId, long personId)
        {
            MailRequest mail = await GenerateFollowUpMailRequest(meetingId, personId);
            try
            {
                _mailService.SendMailAsync(mail);
                await MarkAsSendedFollowUpAsync(meetingId);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
        }

        private async Task MarkAsSendedFollowUpAsync(Guid meetingId) => await _meetingService.MarkAsSendedFollowUpAndFillActualDateAsync(meetingId, DateTime.Now);

        private async Task<MailRequest> GenerateFollowUpMailRequest(Guid meetingId, long personId)
        {
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _personService.GetPersonByIdAsync(personId);
            mail.ToEmail = destinationPerson.Email;
            mail.Body = await GenerateFollowUpAsync(meetingId, personId);
            mail.Subject = "1-2-1 Follow-up";

            return mail;
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectsAsync()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = await _authService.GetUserProjectsAsync((long)userId);
                people = await _personService.GetPeopleFilteredByProjectsAsync(projects);
            }

            return people;
        }

        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            List<Person> people = new List<Person>();

            if (personId != null)
                people.Add(await _personService.GetPersonByIdAsync((long)personId));
            else
                people = await GetPeopleFilteredByProjectsAsync();

            foreach (Person p in people)
            {
                List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsByPersonAsync(p.PersonId);
                foreach (MeetingGoal goal in goals.Where(g => g.IsCompleted == false))
                {
                    taskList.Add(new TaskDTO()
                    {
                        MeetingGoalId = goal.MeetingGoalId,
                        IsCompleted = goal.IsCompleted,
                        MeetingGoalDescription = goal.MeetingGoalDescription,
                        PersonName = p.LastName + " " + p.FirstName + " " + p.SurName,
                        PersonId = p.PersonId,
                        FactDate = await _meetingService.GetFactMeetingDateByIdAsync(goal.MeetingId)
                    });
                }
            }

            return taskList.OrderByDescending(t => t.FactDate).ToList();
        }

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

        public async Task<List<Meeting>> GetMeetingsAsync(long? personId)
        {
            List<Meeting> meetingsRes = new List<Meeting>();
            long? userId = _authService.GetMyUserId();
            if (userId != null)
            {
                meetingsRes = await _meetingService.GetMeetingsByUserIdAsync((long)userId, personId);
            }

            return meetingsRes
                .OrderByDescending(m => m.Person.LastName)
                .OrderByDescending(m => m.MeetingPlanDate)
                .OrderByDescending(m => m.MeetingDate)
                .ToList();
        }

        public async Task ChangeLocaleAsync(int localeId)
        {
            long? userId = _authService.GetMyUserId();
            if (userId == null)
                return;

            await _authService.ChangeLocaleByUserIdAsync((long)userId, (Locale)localeId);
        }

        public async Task<Meeting> CreateMeetingAsync(MeetingDTO meetingDto)
        {
            Meeting newMeeting = meetingDto.ToEntity();
            long? userId = _authService.GetMyUserId();
            if (userId == null)
                throw new DataFoundException("User not found");

                newMeeting.UserId = (long)userId;
            return await _meetingService.CreateMeetingAsync(newMeeting);
        }
    }
}

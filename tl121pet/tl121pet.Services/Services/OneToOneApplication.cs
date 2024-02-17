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
            result += await GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(meetingId, personId);
            result += await GetPreviousMeetingUnclosedGoals(meetingId, personId);
            result += "Если что-то упустил - обязательно сообщи мне об этом!";
            return result;
        }

        public async Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId)
        {
            string prevNotes = "";
            Guid? previousMeetingGuid = await _meetingService.GetPreviousMeetingIdAsync(meetingId, personId);
            if (previousMeetingGuid != null)
            {
                prevNotes = await GetMeetingFeedbackRequiredNotesByMeetingId((Guid)previousMeetingGuid);
            }
            return prevNotes;
        }       

        //TODO: на вход должнен подаваться айдишка встречи и уже готовая почта
        public async Task SendFollowUpAsync(Guid meetingId, long personId)
        {
            MailRequest mail = await GenerateFollowUpMailRequest(meetingId, personId);
            try
            {
                await _mailService.SendMailAsync(mail);
                await MarkAsSendedFollowUpAsync(meetingId);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
        }

        public async Task SendGreetingMailAsync(long personId)
        {
            MailRequest mail = await GeneratGreetingMailRequest(personId);
            try
            {
                await _mailService.SendMailAsync(mail);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
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

        public async Task ArchivePersonAsync(long personId)
        {
            try
            {
                await _personService.ArchivePersonAsync(personId);
                await _meetingService.CompleteAllPersonGoalsAsync(personId);
            }
            catch { throw new Exception("Failed to archive employee"); }
        }

        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId, Guid? currentMeetingId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            long? userId = _authService.GetMyUserId();

            if(userId == null)
                return taskList;

            taskList = await _meetingService.GetTasksByUserId((long)userId, personId, currentMeetingId);

            return taskList;
        }

        [Obsolete]
        public async Task<List<Meeting>> GetMeetingsAsync(long? personId)
        {
            List<Meeting> meetingsRes = new List<Meeting>();
            long? userId = _authService.GetMyUserId();
            if (userId != null)
            {
                meetingsRes = await _meetingService.GetMeetingsByUserIdAsync((long)userId, personId);
            }

            return meetingsRes;
        }
        public async Task<MeetingPagedResponseDTO> GetPagedMeetingsAsync(MeetingPagedRequestDTO request)
        {
            MeetingPagedResponseDTO response = new MeetingPagedResponseDTO();
            long? userId = _authService.GetMyUserId();
            if (userId != null)
            {
                response = await _meetingService.GetMeetingsByUserIdAsync(request, (long)userId);
            }

            return response;
        }

        public async Task ChangeLocaleAsync(int localeId)
        {
            await _authService.ChangeLocaleByUserIdAsync(GetUserId(), (Locale)localeId);
        }

        public async Task<Meeting> CreateMeetingAsync(MeetingDTO meetingDto)
        {
            Meeting newMeeting = meetingDto.ToEntity();
            newMeeting.UserId = GetUserId();
            return await _meetingService.CreateMeetingAsync(newMeeting);
        }

        public async Task<Meeting> UpdateMeetingAsync(MeetingDTO meetingDto)
        {
            Meeting newMeeting = meetingDto.ToEntity();

            newMeeting.UserId = GetUserId();
            return await _meetingService.UpdateMeetingAsync(newMeeting);
        }

        public async Task<Meeting> CreateMeetingByPersonIdAsync(long personId)
        {
            return await _meetingService.CreateCurrentMeetingByPersonIdAsync(GetUserId(), personId);
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(Guid meetingId, long personId)
        {
            string result = "";
            result += await GetMeetingFeedbackRequiredNotesByMeetingId(meetingId);
            result += await GetMeetingGoalsByMeetingId(meetingId);

            return result;
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesByMeetingId(Guid meetingId)
        {
            string meetingNotes = "";
            List<MeetingNote> notes = await _meetingService.GetMeetingFeedbackRequiredNotesAsync(meetingId);
            if (notes.Count() > 0)
            {
                meetingNotes += "На встрече обсудили:\n";
                foreach (MeetingNote mn in notes)
                {
                    meetingNotes += $"\t- {mn.MeetingNoteContent};\n";
                }
                meetingNotes += "\n\n";
            }

            return meetingNotes;
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
                meetingGoals += "\n\n";
            }

            return meetingGoals;
        }

        private async Task<string> GetPreviousMeetingUnclosedGoals(Guid meetingId, long personId)
        {
            string meetingGoals = "";
            List<MeetingGoal> goals = await _meetingService.GetPrevoiusUnclosedMeetingGoalsAsync(meetingId, personId);
            if (goals.Count() > 0)
            {
                meetingGoals += "С прошлой встречи остались цели:\n";
                foreach (MeetingGoal mg in goals)
                {
                    meetingGoals += $"\t- {mg.MeetingGoalDescription};\n";
                }
                meetingGoals += "\n\n";
            }

            return meetingGoals;
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

        private async Task<string> GenerateGreetingMessageAsync(Person targetPerson)
        {
            string result = "";
            result = $"{(!String.IsNullOrEmpty(targetPerson.ShortName) ? targetPerson.ShortName : targetPerson.FirstName)}, привет!\n\n";
            result += $"Это приветственное письмо, которое необходимо, что бы убедиться, что именно ты ({targetPerson.LastName + " " + targetPerson.FirstName}) получишь follow-up!\n";
            result += $"При получении этого письма дай мне знать, что оно получено\n";
            return result;
        }

        private async Task<MailRequest> GeneratGreetingMailRequest(long personId)
        {
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _personService.GetPersonByIdAsync(personId);
            mail.ToEmail = destinationPerson.Email;
            mail.Body = await GenerateGreetingMessageAsync(destinationPerson);
            mail.Subject = "Greeting message";

            return mail;
        }

        private long GetUserId()
        {
            long? userId = _authService.GetMyUserId();
            if (userId == null)
                throw new DataFoundException("User not found");
            else 
                return (long)userId;
        }
    }
}

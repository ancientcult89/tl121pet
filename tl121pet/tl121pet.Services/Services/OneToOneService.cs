using tl121pet.Entities.Aggregate;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{

    public class OneToOneService : IOneToOneService
    {
        private IPersonService _personService;
        private IMeetingService _meetingService;
        private IMailService _mailService;

        public OneToOneService(IMeetingService meetingService, IMailService mailService, IPersonService personService)
        {
            _meetingService = meetingService;
            _mailService = mailService;
            _personService = personService;
        }

        //TODO: дедлайн знает о репозитории людей и встреч. переделать
        //, пусть принимает айдишку? или последний 1-2-1? сильно завязано на репозитории,
        //может вынести в датаконтракт, к-й прилетает из контроллера? или принимаем на вход пользователя и встречу, что облегчит написание теста?
        public async Task<List<OneToOneDeadline>> GetDeadLinesAsync()
        {
            List<OneToOneDeadline> deadLines = new List<OneToOneDeadline>();

            foreach (Person p in await _personService.GetPeopleAsync())
            {
                AlertLevel alert = AlertLevel.None;
                TimeSpan datediff = new TimeSpan();
                Meeting lastMeeting = await _meetingService.GetLastOneToOneByPersonIdAsync(p.PersonId) ?? new Meeting();

                (alert, datediff) = GetOneToOneDeadlineAlertLevel(lastMeeting);

                deadLines.Add(new OneToOneDeadline
                {
                    Person = p,
                    LastMeetingOneToOne = lastMeeting,
                    LastOneToOneMeetingDate = lastMeeting.MeetingDate ?? DateTime.Now,
                    AlertLVL = alert,
                    DayToDeadline = datediff.Days
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

        //TODO: в новой версии фронтенда не понадобится, удалить после перехода
        private (AlertLevel, TimeSpan) GetOneToOneDeadlineAlertLevel(Meeting lastMeeting)
        {
            AlertLevel alert = AlertLevel.None;
            TimeSpan datediff = new TimeSpan();
            if (lastMeeting.MeetingDate == null && lastMeeting.MeetingId == Guid.Empty)
            {
                alert = AlertLevel.High;
            }
            else if (lastMeeting.MeetingDate != null)
            {
                DateTime lastMeetingDate = (DateTime)lastMeeting.MeetingDate;
                datediff = lastMeetingDate.AddMonths(1).Date - DateTime.Now.Date;
                if (datediff.Days < 10 && datediff.Days >= 5)
                    alert = AlertLevel.Low;
                if (datediff.Days < 5)
                    alert = AlertLevel.Normal;
                if (datediff.Days <= 0)
                    alert = AlertLevel.High;
            }
            return (alert, datediff);
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
            catch (Exception ex) { throw new Exception("e-mail service is unavalable"); }
        }

        private async Task MarkAsSendedFollowUpAsync(Guid meetingId) => await _meetingService.MarkAsSendedFollowUpAndFillActualDateAsync(meetingId);

        private async Task<MailRequest> GenerateFollowUpMailRequest(Guid meetingId, long personId)
        {
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _personService.GetPersonByIdAsync(personId);
            mail.ToEmail = destinationPerson.Email;
            mail.Body = await GenerateFollowUpAsync(meetingId, personId);
            mail.Subject = "1-2-1 Follow-up";

            return mail;
        }
    }
}

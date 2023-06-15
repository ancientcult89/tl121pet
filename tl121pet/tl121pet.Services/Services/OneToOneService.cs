using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{

    public class OneToOneService : IOneToOneService
    {
        private IPeopleRepository _peopleRepository;
        private IMeetingRepository _meetingRepository;
        private IMailService _mailService;
        private readonly IPersonService _personService;

        public OneToOneService(IPeopleRepository peopleRepository, IMeetingRepository meetingRepository, IMailService mailService, IPersonService personService)
        { 
            _meetingRepository = meetingRepository;
            _peopleRepository = peopleRepository;
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
                Meeting lastMeeting = _meetingRepository.GetLastOneToOneByPersonId(p.PersonId) ?? new Meeting();

                (alert, datediff) = GetOneToOneDeadlineAlertLevel(lastMeeting);

                deadLines.Add(new OneToOneDeadline {
                    Person = p,
                    LastMeetingOneToOne = lastMeeting,
                    LastOneToOneMeetingDate = lastMeeting.MeetingDate ?? DateTime.Now,
                    AlertLVL = alert,
                    DayToDeadline = datediff.Days
                });
            }
            return deadLines;
        }

        //TODO: рефакторим следующее: на вход подаём объекты пользователя, заметок и целей, внутри метода их не вычисляем!!!
        public async Task<string> GenerateFollowUpAsync(Guid meetingId, long personId)
        {
            string result = "";
            Person person = await _peopleRepository.GetPersonAsync(personId);
            result = $"{(!String.IsNullOrEmpty(person.ShortName) ? person.ShortName : person.FirstName)}, спасибо за проведённую встречу!\n\n";
            result += await GetMeetingNoteAndGoalsAsync(meetingId);
            result += "\n\nЕсли что-то упустил - обязательно сообщи мне об этом!";
            return result;
        }

        public async Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId)
        {
            string prevNoteAndGoals = "";
            Guid? previousMeetingGuid = _meetingRepository.GetPreviousMeetingId(meetingId, personId);
            if (previousMeetingGuid != null)
            {
                prevNoteAndGoals = await GetMeetingNoteAndGoalsAsync((Guid)previousMeetingGuid);
            }
            return prevNoteAndGoals;
        }

        public async Task<string> GetMeetingNoteAndGoalsAsync(Guid meetingId)
        {
            string result = "";
            List<MeetingNote> notes = _meetingRepository.GetMeetingFeedbackRequiredNotes(meetingId);
            if (notes.Count() > 0)
            {
                result += "На встрече обсудили:\n";
                foreach (MeetingNote mn in notes)
                {
                    result += $"\t- {mn.MeetingNoteContent};\n";
                }
            }
            result += "\n\n";
            List<MeetingGoal> goals = _meetingRepository.GetMeetingGoals(meetingId);
            if (goals.Count() > 0)
            {
                result += "К следующему 1-2-1 договорились:\n";
                foreach (MeetingGoal mg in goals)
                {
                    result += $"\t- {mg.MeetingGoalDescription};\n";
                }
            }

            return result;
        }

        public (AlertLevel, TimeSpan) GetOneToOneDeadlineAlertLevel(Meeting lastMeeting)
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
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _peopleRepository.GetPersonAsync(personId);
            string personMail = destinationPerson.Email;
            mail.ToEmail = personMail;
            mail.Body = await GenerateFollowUpAsync(meetingId, personId);
            mail.Subject = "1-2-1 Follow-up";
            try
            {
                _mailService.SendMailAsync(mail);
            }
            finally
            {
                MarkAsSendedFollowUp(meetingId);
            }
        }

        public void MarkAsSendedFollowUp(Guid meetingId) => _meetingRepository.MarAsSendedFollowUp(meetingId);
    }
}

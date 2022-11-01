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
        public OneToOneService(IPeopleRepository peopleRepository, IMeetingRepository meetingRepository)
        { 
            _meetingRepository = meetingRepository;
            _peopleRepository = peopleRepository;
        }
        public List<OneToOneDeadline> GetDeadLines()
        {
            List<OneToOneDeadline> deadLines = new List<OneToOneDeadline>();
            foreach (Person p in _peopleRepository.GetPeople())
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
                    DayToDeadline = datediff.Days > 0 ? datediff.Days : 0
                });
            }
            return deadLines;
        }

        public string GenerateFollowUp(Guid meetingId, long personId)
        {
            string result = "";
            result = $"{_peopleRepository.GetPerson(personId).FirstName}, спасибо за проведённую встречу!\n\n" ;
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
            result += "\n\nЕсли что-то упустил - обязательно сообщи мне об этом!";
            return result;
        }

        public (AlertLevel, TimeSpan) GetOneToOneDeadlineAlertLevel(Meeting lastMeeting)
        {
            AlertLevel alert = AlertLevel.None;
            TimeSpan datediff = new TimeSpan();
            if (lastMeeting.MeetingDate == null)
            {
                alert = AlertLevel.High;
            }
            else if (lastMeeting != null)
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
    }
}

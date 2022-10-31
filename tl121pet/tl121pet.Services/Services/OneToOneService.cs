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
                Meeting lastMeeting = _meetingRepository.GetLastOneToOneByPersonId(p.PersonId) ?? new Meeting();

                //calculate alrt level
                TimeSpan datediff = new TimeSpan();
                if (lastMeeting.MeetingDate == null)
                {
                    alert = AlertLevel.High;
                }
                else if (lastMeeting != null)
                {
                    DateTime lastMeetingDate = (DateTime)lastMeeting.MeetingDate;
                    datediff = lastMeetingDate.AddMonths(1).Date - DateTime.Now.Date;
                    if(datediff.Days < 10 && datediff.Days >= 5)
                        alert = AlertLevel.Low;
                    if(datediff.Days < 5)
                        alert = AlertLevel.Normal;
                    if(datediff.Days <= 0)
                        alert = AlertLevel.High;
                }

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
    }
}

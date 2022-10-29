using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{

    public class OneToOneDeadlineService : IOneToOneDeadlineService
    {
        private IPeopleRepository _peopleRepository;
        private IMeetingRepository _meetingRepository;
        public OneToOneDeadlineService(IPeopleRepository peopleRepository, IMeetingRepository meetingRepository)
        { 
            _meetingRepository = meetingRepository;
            _peopleRepository = peopleRepository;
        }
        public List<OneToOneDeadline> GetDeadLines()
        {
            List<OneToOneDeadline> deadLines = new List<OneToOneDeadline>();
            foreach (Person p in _peopleRepository.GetPeople())
            {
                Meeting lastMeeting = _meetingRepository.GetLastOneToOneByPersonId(p.PersonId) ?? new Meeting();
                deadLines.Add(new OneToOneDeadline {
                    Person = p,
                    LastMeetingOneToOne = lastMeeting,
                    LastOneToOneMeetingDate = lastMeeting.MeetingDate ?? DateTime.Now
                });
            }
            return deadLines;
        }
    }
}

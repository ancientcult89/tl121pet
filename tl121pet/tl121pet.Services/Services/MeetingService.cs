using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class MeetingService : IMeetingService
    {

        private readonly IAuthService _authService;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IAdminRepository _adminRepository;

        public MeetingService(IAuthService authService
            , IPeopleRepository peopleRepository
            , IMeetingRepository meetingRepository
            , IAdminRepository adminRepository)
        {
            _authService = authService;
            _peopleRepository = peopleRepository;
            _meetingRepository = meetingRepository;
            _adminRepository = adminRepository;
        }
        public async Task<List<Meeting>> GetMeetingsAsync(long? personId)
        {
            List<Meeting> meetingsRes = new List<Meeting>();
            long? userId = _authService.GetMyUserId();
            if (userId != null)
            {
                List<Person> people = new List<Person>();
                List<ProjectTeam> projects = new List<ProjectTeam>();
                projects = await _adminRepository.GetUserProjectsAsync((long)userId);
                people = GetPeopleByProjects(projects, personId);
                meetingsRes = GetMeetingsByPerson(people);
            }

            return meetingsRes
                .OrderByDescending(m => m.Person.LastName)
                .OrderByDescending(m => m.MeetingPlanDate)
                .OrderByDescending(m => m.MeetingDate)
                .ToList();
        }

        private List<Person> GetPeopleByProjects(List<ProjectTeam> projects, long? personId)
        {
            List<Person> personByProjects = new List<Person>();

            foreach (ProjectTeam pt in projects)
            {
                personByProjects.AddRange(_peopleRepository.GetPeopleFilteredByProject(pt.ProjectTeamId));
            }

            if (personId != null)
                personByProjects = personByProjects.Where(p => p.PersonId == (long)personId).ToList();

            return personByProjects.Distinct(new PersonComparer()).ToList();
        }

        private List<Meeting> GetMeetingsByPerson(List<Person> people)
        {
            List<Meeting> meetingsByPerson = new List<Meeting>();
            foreach (Person person in people)
            {
                meetingsByPerson.AddRange(_meetingRepository.GetMeetingsByPersonId(person.PersonId));
            }
            return meetingsByPerson;
        }
    }
}

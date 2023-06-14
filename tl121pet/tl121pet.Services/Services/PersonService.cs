using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private readonly IAuthService _authService;
        private readonly IAdminRepository _adminRepository;
        private readonly IPeopleRepository _peopleRepository;

        public PersonService(IAuthService authService, IAdminRepository adminRepository, IPeopleRepository peopleRepository)
        {
            _authService = authService;
            _adminRepository = adminRepository;
            _peopleRepository = peopleRepository;
        }

        public async Task<List<PersonInitials>> GetInitialsAsync()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            List<Person> people = await GetPeopleAsync();
            personInitials = (List<PersonInitials>)people.Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.LastName + " " + p.FirstName + " " + p.SurName
                }).ToList();
            return personInitials;
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = _adminRepository.GetUserProjects((long)userId);
                people = GetPeopleFilteredByProjects(projects);
            }

            return people;
        }

        private List<Person> GetPeopleFilteredByProjects(List<ProjectTeam> projects)
        {
            List<Person> peopleFiltered = new List<Person>();
            foreach (ProjectTeam pt in projects)
            {
                peopleFiltered.AddRange(_peopleRepository.GetPeopleFilteredByProject(pt.ProjectTeamId));
            }

            peopleFiltered = peopleFiltered.Distinct(new PersonComparer()).ToList();

            return peopleFiltered;
        }
    }
}

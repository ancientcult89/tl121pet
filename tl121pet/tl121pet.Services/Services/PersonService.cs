using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;
        private readonly IAdminRepository _adminRepository;
        private readonly IPeopleRepository _peopleRepository;

        public PersonService(DataContext dataContext, IAuthService authService, IAdminRepository adminRepository, IPeopleRepository peopleRepository)
        {
            _dataContext = dataContext;
            _authService = authService;
            _adminRepository = adminRepository;
            _peopleRepository = peopleRepository;
        }

        public List<PersonInitials> GetInitials()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            personInitials = (List<PersonInitials>)GetPeople().Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.LastName + " " + p.FirstName + " " + p.SurName
                }).ToList();
            return personInitials;
        }

        public List<Person> GetPeople()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = _adminRepository.GetUserProjects((long)userId);
                foreach (ProjectTeam pt in projects)
                {
                    people.AddRange(_peopleRepository.GetPeopleFilteredByProject(pt.ProjectTeamId));
                }
            }

            return people;
        }
    }
}

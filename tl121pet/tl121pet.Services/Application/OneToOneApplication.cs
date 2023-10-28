using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Application
{
    public class OneToOneApplication
    {
        private readonly IAuthService _authService;
        private readonly IPersonService _personService;
        private readonly IProjectService _projectService;

        public OneToOneApplication(IPersonService personService, IAuthService authService, IProjectService projectService)
        {
            _authService = authService;
            _personService = personService;
            _projectService = projectService;
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

        [Obsolete]
        public async Task<List<ProjectTeam>> GetFilteredProjectsAsync()
        {
            long? userId = _authService.GetMyUserId();
            return await _projectService.GetFilteredProjectsAsync(userId);
        }


        [Obsolete]
        public async Task<List<PersonInitials>> GetInitialsAsync()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            List<Person> people = await GetPeopleFilteredByProjectsAsync();
            personInitials = (List<PersonInitials>)people.Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.LastName + " " + p.FirstName + " " + p.SurName
                }).ToList();
            return personInitials;
        }
    }
}

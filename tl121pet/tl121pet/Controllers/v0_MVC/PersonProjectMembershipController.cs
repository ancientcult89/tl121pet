using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize]
    public class PersonProjectMembershipController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IPersonService _personService;

        public PersonProjectMembershipController(IProjectService projectService, IPersonService personService)
        {
            _projectService = projectService;
            _personService = personService;
        }
        public async Task<IActionResult> PersonProjectMemberList()
        {
            //TODO: зоны ответсвенности контроллера: 1. отобразить список сотрудников с их проектами. 2. сформировать список. Нужна декомпозиция
            List<ProjectMemberDTO> projectMembers = new List<ProjectMemberDTO>();
            List<Person> people = await _personService.GetAllPeopleAsync();
            foreach (Person person in people)
            {
                string projects = await _projectService.GetPersonsProjectsAsync(person.PersonId);
                projectMembers.Add(new ProjectMemberDTO() {
                    PersonId = person.PersonId,
                    PersonName = person.FirstName + " " + person.LastName,
                    Projects = projects
                });
            }
            return View("ProjectMemberList", projectMembers);
        }

        public async Task<IActionResult> Details(long id)
        {
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM() {
                SelectedItem = await _personService.GetPersonByIdAsync(id),
                ProjectTeams = await _projectService.GetPersonMembershipAsync(id),
                Mode = FormMode.Details
            };
            return View("ProjectMemberEditor", vm);
        }

        public async Task<IActionResult> Edit(long id)
        {
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = await _personService.GetPersonByIdAsync(id),
                ProjectTeams = await _projectService.GetPersonMembershipAsync(id),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembership([FromForm] ProjectMemberEditFormVM vm, long personId)
        {
            if(ModelState.IsValid)
                await _projectService.AddPersonMembershipAsync(personId, vm.NewProjectTeamId);

            vm.SelectedItem = await _personService.GetPersonByIdAsync(personId);
            vm.ProjectTeams = await _projectService.GetPersonMembershipAsync(personId);
            vm.Mode = FormMode.Edit;

            return View("ProjectMemberEditor", vm);
        }

        public async Task<IActionResult> DeleteMembership(long ptId, long personId)
        {
            await _projectService.DeletePersonMembershipAsync(personId, ptId);
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = await _personService.GetPersonByIdAsync(personId),
                ProjectTeams = await _projectService.GetPersonMembershipAsync(personId),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }
    }
}

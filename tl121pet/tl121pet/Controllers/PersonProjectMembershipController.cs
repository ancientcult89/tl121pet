using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class PersonProjectMembershipController : Controller
    {
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IPersonService _personService;

        public PersonProjectMembershipController(IProjectTeamRepository projectTeamRepository, IPersonService personService)
        {
            _projectTeamRepository = projectTeamRepository;
            _personService = personService;
        }
        public async Task<IActionResult> PersonProjectMemberList()
        {
            //TODO: зоны ответсвенности контроллера: 1. отобразить список сотрудников с их проектами. 2. сформировать список. Нужна декомпозиция
            List<ProjectMemberDTO> projectMembers = new List<ProjectMemberDTO>();
            List<Person> people = await _personService.GetAllPeopleAsync();
            foreach (Person person in people)
            {
                string projects = await _projectTeamRepository.GetPersonsProjectsAsync(person.PersonId);
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
                SelectedItem = await _personService.GetPersonAsync(id),
                ProjectTeams = await _projectTeamRepository.GetPersonMembershipAsync(id),
                Mode = FormMode.Details
            };
            return View("ProjectMemberEditor", vm);
        }

        public async Task<IActionResult> Edit(long id)
        {
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = await _personService.GetPersonAsync(id),
                ProjectTeams = await _projectTeamRepository.GetPersonMembershipAsync(id),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembership([FromForm] ProjectMemberEditFormVM vm, long personId)
        {
            if(ModelState.IsValid)
                await _projectTeamRepository.AddPersonMembershipAsync(personId, vm.NewProjectTeamId);

            vm.SelectedItem = await _personService.GetPersonAsync(personId);
            vm.ProjectTeams = await _projectTeamRepository.GetPersonMembershipAsync(personId);
            vm.Mode = FormMode.Edit;

            return View("ProjectMemberEditor", vm);
        }

        public async Task<IActionResult> DeleteMembership(long ptId, long personId)
        {
            await _projectTeamRepository.DeletePersonMembershipAsync(personId, ptId);
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = await _personService.GetPersonAsync(personId),
                ProjectTeams = await _projectTeamRepository.GetPersonMembershipAsync(personId),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }
    }
}

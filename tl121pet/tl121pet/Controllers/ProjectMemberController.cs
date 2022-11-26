using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class ProjectMemberController : Controller
    {
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IPeopleRepository _peopleRepository;

        public ProjectMemberController(IProjectTeamRepository projectTeamRepository, IPeopleRepository peopleRepository)
        {
            _projectTeamRepository = projectTeamRepository;
            _peopleRepository = peopleRepository;
        }
        public IActionResult ProjectMemberList()
        {
            List<ProjectMemberDTO> projectMembers = new List<ProjectMemberDTO>();
            List<Person> people = _peopleRepository.GetPeople();
            foreach (Person person in people)
            {
                string projects = _projectTeamRepository.GetPersonsProjects(person.PersonId);
                projectMembers.Add(new ProjectMemberDTO() {
                    PersonId = person.PersonId,
                    PersonName = person.FirstName + " " + person.LastName,
                    Projects = projects
                });
            }
            return View("ProjectMemberList", projectMembers);
        }

        public IActionResult Details(long id)
        {
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM() {
                SelectedItem = _peopleRepository.GetPerson(id)
                , ProjectTeams = _projectTeamRepository.GetPersonMembership(id)
                , Mode = FormMode.Details
            };
            return View("ProjectMemberEditor", vm);
        }

        public IActionResult Edit(long id)
        {
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = _peopleRepository.GetPerson(id),
                ProjectTeams = _projectTeamRepository.GetPersonMembership(id),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }

        [HttpPost]
        public IActionResult AddMembership([FromForm] ProjectMemberEditFormVM vm, long personId)
        {
            _projectTeamRepository.AddPersonMembership(personId, vm.NewProjectTeamId);

            vm.SelectedItem = _peopleRepository.GetPerson(personId);
            vm.ProjectTeams = _projectTeamRepository.GetPersonMembership(personId);
            vm.Mode = FormMode.Edit;

            return View("ProjectMemberEditor", vm);
        }

        public IActionResult DeleteMembership(long ptId, long personId)
        {
            _projectTeamRepository.DeletePersonMembership(personId, ptId);
            ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
            {
                SelectedItem = _peopleRepository.GetPerson(personId),
                ProjectTeams = _projectTeamRepository.GetPersonMembership(personId),
                Mode = FormMode.Edit
            };
            return View("ProjectMemberEditor", vm);
        }
    }
}

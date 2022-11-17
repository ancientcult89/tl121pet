using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
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
                SelectedItem = id
                , ProjectTeams = _projectTeamRepository.GetPersonMembership(id)
                , Mode = FormMode.Details
            };
            return View("ProjectMemberEditor", vm);
        }

        //public IActionResult Edit(long id)
        //{
        //    return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { SelectedItem = _projectTeamRepository.GetProjectTeamById(id) ?? new ProjectTeam(), Mode = FormMode.Edit });
        //}

        //[HttpPost]
        //public IActionResult Edit([FromForm] SimpleEditFormVM<ProjectTeam> ptVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _projectTeamRepository.UpdateProjectTeam(ptVM.SelectedItem);
        //        return View("ProjectTeamEditor", ptVM);
        //    }
        //    return View("ProjectTeamEditor", ptVM);
        //}
    }
}

using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Controllers
{
    public class UserProjectMembershipController : Controller
    {
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IAdminRepository _adminRepository;

        public UserProjectMembershipController(IProjectTeamRepository projectTeamRepository, IAdminRepository adminRepository)
        {
            _projectTeamRepository = projectTeamRepository;
            _adminRepository = adminRepository;
        }
        public IActionResult UserProjectMemberList()
        {
            List<UserProjectMemberDTO> userProjectMembers = new List<UserProjectMemberDTO>();
            List<User> users = _adminRepository.GetUserList();
            foreach (User user in users)
            {
                string projects = _projectTeamRepository.GetPersonsProjects(user.Id);
                userProjectMembers.Add(new UserProjectMemberDTO()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Projects = projects
                });
            }
            return View("UserProjectMemberList", userProjectMembers);
        }

        //public IActionResult Details(long id)
        //{
        //    ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
        //    {
        //        SelectedItem = _peopleRepository.GetPerson(id)
        //        ,
        //        ProjectTeams = _projectTeamRepository.GetPersonMembership(id)
        //        ,
        //        Mode = FormMode.Details
        //    };
        //    return View("ProjectMemberEditor", vm);
        //}

        //public IActionResult Edit(long id)
        //{
        //    ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
        //    {
        //        SelectedItem = _peopleRepository.GetPerson(id),
        //        ProjectTeams = _projectTeamRepository.GetPersonMembership(id),
        //        Mode = FormMode.Edit
        //    };
        //    return View("ProjectMemberEditor", vm);
        //}

        //[HttpPost]
        //public IActionResult AddMembership([FromForm] ProjectMemberEditFormVM vm, long personId)
        //{
        //    _projectTeamRepository.AddMembership(personId, vm.NewProjectTeamId);

        //    vm.SelectedItem = _peopleRepository.GetPerson(personId);
        //    vm.ProjectTeams = _projectTeamRepository.GetPersonMembership(personId);
        //    vm.Mode = FormMode.Edit;

        //    return View("ProjectMemberEditor", vm);
        //}

        //public IActionResult DeleteMembership(long ptId, long personId)
        //{
        //    _projectTeamRepository.DeleteMembership(personId, ptId);
        //    ProjectMemberEditFormVM vm = new ProjectMemberEditFormVM()
        //    {
        //        SelectedItem = _peopleRepository.GetPerson(personId),
        //        ProjectTeams = _projectTeamRepository.GetPersonMembership(personId),
        //        Mode = FormMode.Edit
        //    };
        //    return View("ProjectMemberEditor", vm);
        //}
    }
}

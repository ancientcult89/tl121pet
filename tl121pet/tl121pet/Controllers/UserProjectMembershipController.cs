using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize(Roles = "Admin")]
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
                string projects = _projectTeamRepository.GetUserProjects(user.Id);
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

        public IActionResult Details(long id)
        {
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = _adminRepository.GetUserById(id),
                ProjectTeams = _projectTeamRepository.GetUserMembership(id),
                Mode = FormMode.Details
            };
            return View("UserMembershipEditor", vm);
        }

        public IActionResult Edit(long id)
        {
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = _adminRepository.GetUserById(id),
                ProjectTeams = _projectTeamRepository.GetUserMembership(id),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }

        [HttpPost]
        public IActionResult AddMembership([FromForm] UserMemberEditFormVM vm, long userId)
        {
            if(ModelState.IsValid)
                _projectTeamRepository.AddUserMembership(userId, vm.NewProjectTeamId);

            vm.SelectedItem = _adminRepository.GetUserById(userId);
            vm.ProjectTeams = _projectTeamRepository.GetUserMembership(userId);
            vm.Mode = FormMode.Edit;

            return View("UserMembershipEditor", vm);
        }

        public IActionResult DeleteMembership(long ptId, long userId)
        {
            _projectTeamRepository.DeleteUserMembership(userId, ptId);
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = _adminRepository.GetUserById(userId),
                ProjectTeams = _projectTeamRepository.GetUserMembership(userId),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }
    }
}

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
        public async Task<IActionResult> UserProjectMemberList()
        {
            List<UserProjectMemberDTO> userProjectMembers = new List<UserProjectMemberDTO>();
            //TODO: убрать говнокод из контроллера. либо экстеншн либо маппер
            List<User> users = await _adminRepository.GetUserListAsync();
            foreach (User user in users)
            {
                string projects = await _projectTeamRepository.GetUserProjectsNameAsync(user.Id);
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

        public async Task<IActionResult> Details(long id)
        {
            //TODO: очень напрашивается AppLayer
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = await _adminRepository.GetUserByIdAsync(id),
                ProjectTeams = await _projectTeamRepository.GetUserMembershipAsync(id),
                Mode = FormMode.Details
            };
            return View("UserMembershipEditor", vm);
        }

        public async Task<IActionResult> Edit(long id)
        {
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = await _adminRepository.GetUserByIdAsync(id),
                ProjectTeams = await _projectTeamRepository.GetUserMembershipAsync(id),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembership([FromForm] UserMemberEditFormVM vm, long userId)
        {
            if(ModelState.IsValid)
                await _projectTeamRepository.AddUserMembershipAsync(userId, vm.NewProjectTeamId);

            vm.SelectedItem = await _adminRepository.GetUserByIdAsync(userId);
            vm.ProjectTeams = await _projectTeamRepository.GetUserMembershipAsync(userId);
            vm.Mode = FormMode.Edit;

            return View("UserMembershipEditor", vm);
        }

        public async Task<IActionResult> DeleteMembership(long ptId, long userId)
        {
            await _projectTeamRepository.DeleteUserMembershipAsync(userId, ptId);
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = await _adminRepository.GetUserByIdAsync(userId),
                ProjectTeams = await _projectTeamRepository.GetUserMembershipAsync(userId),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }
    }
}

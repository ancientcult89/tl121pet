using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProjectMembershipController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;

        public UserProjectMembershipController(IProjectService projectService, IAuthService authService)
        {
            _projectService = projectService;
            _authService = authService;
        }
        public async Task<IActionResult> UserProjectMemberList()
        {
            List<UserProjectMemberDTO> userProjectMembers = new List<UserProjectMemberDTO>();
            //TODO: убрать говнокод из контроллера. либо экстеншн либо маппер
            List<User> users = await _authService.GetUserListAsync();
            foreach (User user in users)
            {
                string projects = await _projectService.GetUserProjectsNameAsync(user.Id);
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
                SelectedItem = await _authService.GetUserByIdAsync(id),
                ProjectTeams = await _projectService.GetUserMembershipAsync(id),
                Mode = FormMode.Details
            };
            return View("UserMembershipEditor", vm);
        }

        public async Task<IActionResult> Edit(long id)
        {
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = await _authService.GetUserByIdAsync(id),
                ProjectTeams = await _projectService.GetUserMembershipAsync(id),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembership([FromForm] UserMemberEditFormVM vm, long userId)
        {
            if(ModelState.IsValid)
                await _projectService.AddUserMembershipAsync(userId, vm.NewProjectTeamId);

            vm.SelectedItem = await _authService.GetUserByIdAsync(userId);
            vm.ProjectTeams = await _projectService.GetUserMembershipAsync(userId);
            vm.Mode = FormMode.Edit;

            return View("UserMembershipEditor", vm);
        }

        public async Task<IActionResult> DeleteMembership(long ptId, long userId)
        {
            await _projectService.DeleteUserMembershipAsync(userId, ptId);
            UserMemberEditFormVM vm = new UserMemberEditFormVM()
            {
                SelectedItem = await _authService.GetUserByIdAsync(userId),
                ProjectTeams = await _projectService.GetUserMembershipAsync(userId),
                Mode = FormMode.Edit
            };
            return View("UserMembershipEditor", vm);
        }
    }
}

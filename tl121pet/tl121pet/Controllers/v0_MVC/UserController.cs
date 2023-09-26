using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> UserList()
        {
            return View("UserList", await _authService.GetUserListAsync());
        }

        public async Task<IActionResult> Edit(long id)
        {
            User? user = await _authService.GetUserByIdAsync(id);
            UserDTO userEditRequest = user.ToDto();
            return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                SelectedItem = userEditRequest ?? new UserDTO(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<UserDTO> userEditRequestVM)
        {
            User user = await _authService.GetUserByIdAsync(userEditRequestVM.SelectedItem.Id);
            user.UserName = userEditRequestVM.SelectedItem.UserName;
            user.Email = userEditRequestVM.SelectedItem.Email;
            user.RoleId = userEditRequestVM.SelectedItem.RoleId;

            if (ModelState.IsValid)
            {
                await _authService.UpdateUserAsync(user);
                return View("UserEditor", userEditRequestVM);
            }

            return View("UserEditor", userEditRequestVM);
        }

        public IActionResult Create()
        {
            return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                SelectedItem = new UserDTO(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<UserDTO> userEditRequestVM)
        {            
            if (ModelState.IsValid)
            {
                _authService.CreatePasswordHash(userEditRequestVM.SelectedItem.Password, out byte[] passwordHash, out byte[] passwordSalt);
                User newUser = userEditRequestVM.SelectedItem.ToEntity(passwordHash, passwordSalt);
                await _authService.CreateUserAsync(newUser);

                User? user = await _authService.GetUserByEmailAsync(userEditRequestVM.SelectedItem.Email);
                UserDTO userEditRequest = user.ToDto();

                return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                    SelectedItem = userEditRequest ?? new UserDTO(),
                    Mode = FormMode.Edit });
            }
            userEditRequestVM.Mode = FormMode.Create;
            return View("UserEditor", userEditRequestVM);
        }

        public async Task<IActionResult> Details(long id)
        {
            User? user = await _authService.GetUserByIdAsync(id);
            UserDTO userEditRequest = user.ToDto();

            return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                SelectedItem = userEditRequest ?? new UserDTO(),
                Mode = FormMode.Details });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _authService.DeleteUserAsync(id);
            return RedirectToAction("UserList");
        }

        public IActionResult ChangePassword(long userId)
        {
            return View("ChangePassword", new SimpleEditFormVM<ChangeUserPasswordRequestDTO>() { 
                SelectedItem = new ChangeUserPasswordRequestDTO() { UserId = userId },
                Mode = FormMode.Edit 
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] SimpleEditFormVM<ChangeUserPasswordRequestDTO> changePasswordRequest)
        {
            //TODO: сейчас не понятно - сменили мы пароль или нет
            if (ModelState.IsValid)
            {
                User user = await _authService.GetUserByIdAsync(changePasswordRequest.SelectedItem.UserId);
                if (_authService.VerifyPasswordHash(changePasswordRequest.SelectedItem.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    _authService.CreatePasswordHash(changePasswordRequest.SelectedItem.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }
                await _authService.UpdateUserAsync(user);
                UserDTO userEditRequest = user.ToDto();

                return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                    SelectedItem = userEditRequest ?? new UserDTO(),
                    Mode = FormMode.Edit });
            }
            return View("ChangePassword", changePasswordRequest);
        }
    }
}

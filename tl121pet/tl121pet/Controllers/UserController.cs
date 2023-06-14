using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAuthService _authService;
        private readonly IAutomapperMini _automapperMini;

        public UserController(IAdminRepository adminRepository, IAuthService authService, IAutomapperMini automapperMini)
        {
            _adminRepository = adminRepository;
            _authService = authService;
            _automapperMini = automapperMini;
        }

        public async Task<IActionResult> UserList()
        {
            return View("UserList", await _adminRepository.GetUserListAsync());
        }

        public IActionResult Edit(long id)
        {
            User? user = _adminRepository.GetUserById(id);
            UserDTO userEditRequest = _automapperMini.UserEntityToDto(user);
            return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                SelectedItem = userEditRequest ?? new UserDTO(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<UserDTO> userEditRequestVM)
        {
            User user = _adminRepository.GetUserById(userEditRequestVM.SelectedItem.Id);
            user.UserName = userEditRequestVM.SelectedItem.UserName;
            user.Email = userEditRequestVM.SelectedItem.Email;
            user.RoleId = userEditRequestVM.SelectedItem.RoleId;

            if (ModelState.IsValid)
            {
                await _adminRepository.UpdateUserAsync(user);
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
        public IActionResult Create([FromForm] SimpleEditFormVM<UserDTO> userEditRequestVM)
        {            
            if (ModelState.IsValid)
            {
                _authService.CreatePasswordHash(userEditRequestVM.SelectedItem.Password, out byte[] passwordHash, out byte[] passwordSalt);
                User newUser = _automapperMini.UserDtoToEntity(userEditRequestVM.SelectedItem, passwordHash, passwordSalt);
                _adminRepository.CreateUser(newUser);

                User? user = _adminRepository.GetUserByEmail(userEditRequestVM.SelectedItem.Email);
                UserDTO userEditRequest = _automapperMini.UserEntityToDto(user);

                return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                    SelectedItem = userEditRequest ?? new UserDTO(),
                    Mode = FormMode.Edit });
            }
            userEditRequestVM.Mode = FormMode.Create;
            return View("UserEditor", userEditRequestVM);
        }

        public IActionResult Details(long id)
        {
            User? user = _adminRepository.GetUserById(id);
            UserDTO userEditRequest = _automapperMini.UserEntityToDto(user);

            return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                SelectedItem = userEditRequest ?? new UserDTO(),
                Mode = FormMode.Details });
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _adminRepository.DeleteUser(id);
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
                User user = _adminRepository.GetUserById(changePasswordRequest.SelectedItem.UserId);
                if (_authService.VerifyPasswordHash(changePasswordRequest.SelectedItem.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    _authService.CreatePasswordHash(changePasswordRequest.SelectedItem.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }
                await _adminRepository.UpdateUserAsync(user);
                UserDTO userEditRequest = _automapperMini.UserEntityToDto(user);

                return View("UserEditor", new SimpleEditFormVM<UserDTO>() { 
                    SelectedItem = userEditRequest ?? new UserDTO(),
                    Mode = FormMode.Edit });
            }
            return View("ChangePassword", changePasswordRequest);
        }
    }
}

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
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAuthService _authService;

        public UserController(IAdminRepository adminRepository, IAuthService authService)
        {
            _adminRepository = adminRepository;
            _authService = authService;
        }

        public IActionResult UserList()
        {
            return View("UserList", _adminRepository.GetUserList());
        }

        public IActionResult Edit(long id)
        {
            User? user = _adminRepository.GetUserById(id);
            UserEditRequest userEditRequest = new UserEditRequest { 
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            };
            return View("UserEditor", new SimpleEditFormVM<UserEditRequest>() { SelectedItem = userEditRequest ?? new UserEditRequest(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<UserEditRequest> userEditRequestVM)
        {
            User user = _adminRepository.GetUserById(userEditRequestVM.SelectedItem.Id);
            user.UserName = userEditRequestVM.SelectedItem.UserName;
            user.Email = userEditRequestVM.SelectedItem.Email;
            user.RoleId = userEditRequestVM.SelectedItem.RoleId;

            if (ModelState.IsValid)
            {
                _adminRepository.UpdateUser(user);
                return View("UserEditor", userEditRequestVM);
            }
            return View("UserEditor", userEditRequestVM);
        }

        public IActionResult Create()
        {
            return View("UserEditor", new SimpleEditFormVM<UserEditRequest>() { SelectedItem = new UserEditRequest(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<UserEditRequest> userEditRequestVM)
        {
            _authService.CreatePasswordHash(userEditRequestVM.SelectedItem.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User newUser = new User
            {
                Email = userEditRequestVM.SelectedItem.Email,
                UserName = userEditRequestVM.SelectedItem.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = userEditRequestVM.SelectedItem.RoleId
            };
            if (ModelState.IsValid)
            {
                _adminRepository.CreateUser(newUser);

                User? user = _adminRepository.GetUserByEmail(userEditRequestVM.SelectedItem.Email);
                UserEditRequest userEditRequest = new UserEditRequest
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    RoleId = user.RoleId
                };
                return View("UserEditor", new SimpleEditFormVM<UserEditRequest>() { SelectedItem = userEditRequest ?? new UserEditRequest(), Mode = FormMode.Edit });
            }
            return View("UserEditor", userEditRequestVM);
        }

        public IActionResult Details(long id)
        {
            User? user = _adminRepository.GetUserById(id);
            UserEditRequest userEditRequest = new UserEditRequest
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId
            };
            return View("UserEditor", new SimpleEditFormVM<UserEditRequest>() { SelectedItem = userEditRequest ?? new UserEditRequest(), Mode = FormMode.Details });
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _adminRepository.DeleteUser(id);
            return RedirectToAction("UserList");
        }

        public IActionResult ChangePassword(long userId)
        {
            return View("ChangePassword", new SimpleEditFormVM<ChangeUserPasswordRequest>() { 
                SelectedItem = new ChangeUserPasswordRequest() { UserId = userId }, Mode = FormMode.Edit 
            });
        }

        [HttpPost]
        public IActionResult ChangePassword([FromForm] SimpleEditFormVM<ChangeUserPasswordRequest> changePasswordRequest)
        {
            if (ModelState.IsValid)
            {
                User user = _adminRepository.GetUserById(changePasswordRequest.SelectedItem.UserId);
                if (_authService.VerifyPasswordHash(changePasswordRequest.SelectedItem.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    _authService.CreatePasswordHash(changePasswordRequest.SelectedItem.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }
                _adminRepository.UpdateUser(user);
                UserEditRequest userEditRequest = new UserEditRequest
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    RoleId = user.RoleId
                };
                return View("UserEditor", new SimpleEditFormVM<UserEditRequest>() { SelectedItem = userEditRequest ?? new UserEditRequest(), Mode = FormMode.Edit });
            }
            return View("ChangePassword", changePasswordRequest);
        }
    }
}

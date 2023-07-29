﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        //TODO: избавиться от зависимости слоя данных в контроллере
        private DataContext _dataContext;
        private readonly IAuthService _authService;

        public RoleController(DataContext dataContext, IAuthService authService)
        {

            _dataContext = dataContext;
            _authService = authService;
        }

        public IActionResult RoleList()
        {
            return View("RoleList", _dataContext.Roles.ToList());
        }

        public IActionResult Edit(int id)
        {
            return View("RoleEditor", new SimpleEditFormVM<Role>() { 
                SelectedItem = _dataContext.Roles.Find(id) ?? new Role(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Role> roleVM)
        {
            if (ModelState.IsValid && roleVM.SelectedItem.RoleName != "Admin")
            {
                await _authService.UpdateRoleAsync(roleVM.SelectedItem);
                return View("RoleEditor", roleVM);
            }
            return View("RoleEditor", roleVM);
        }

        public IActionResult Details(int id)
        {
            return View("RoleEditor", new SimpleEditFormVM<Role>() { 
                SelectedItem = _dataContext.Roles.Find(id) ?? new Role(),
                Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("RoleEditor", new SimpleEditFormVM<Role>() { 
                SelectedItem = new Role(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<Role> roleVM)
        {
            if (ModelState.IsValid)
            {
                await _authService.CreateRoleAsync(roleVM.SelectedItem);
                roleVM.Mode = FormMode.Edit;
                return View("RoleEditor", roleVM);
            }
            roleVM.Mode = FormMode.Create;
            return View("RoleEditor", roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _authService.DeleteRoleAsync(id);
            return RedirectToAction("RoleList");
        }
    }
}

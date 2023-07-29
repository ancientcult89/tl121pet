using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class ProjectTeamController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectTeamController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        public async Task<IActionResult> ProjectTeamList()
        {
            return View("ProjectTeamList", await _projectService.GetAllTeamsAsync());
        }

        public async Task<IActionResult> Edit(long id)
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { 
                SelectedItem = await _projectService.GetProjectTeamByIdAsync(id) ?? new ProjectTeam(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<ProjectTeam> ptVM)
        {
            if (ModelState.IsValid)
            {
                await _projectService.UpdateProjectTeamAsync(ptVM.SelectedItem);
                return View("ProjectTeamEditor", ptVM);
            }
            return View("ProjectTeamEditor", ptVM);
        }

        public async Task<IActionResult> Details(long id)
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { 
                SelectedItem = await _projectService.GetProjectTeamByIdAsync(id) ?? new ProjectTeam(),
                Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { 
                SelectedItem = new ProjectTeam(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<ProjectTeam> ptVM)
        {
            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectTeamAsync(ptVM.SelectedItem);
                ptVM.Mode = FormMode.Edit;
                return View("ProjectTeamEditor", ptVM);
            }
            ptVM.Mode = FormMode.Create;
            return View("ProjectTeamEditor", ptVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _projectService.DeleteProjectTeamAsync(id);
            return RedirectToAction("ProjectTeamList");
        }
    }
}

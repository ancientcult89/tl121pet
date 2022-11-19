using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class ProjectTeamController : Controller
    {
        private readonly IProjectTeamRepository _projectTeamRepository;

        public ProjectTeamController(IProjectTeamRepository projectTeamRepository)
        {
            _projectTeamRepository = projectTeamRepository;
        }
        public IActionResult ProjectTeamList()
        {
            return View("ProjectTeamList", _projectTeamRepository.GetAllTeams());
        }

        public IActionResult Edit(long id)
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { SelectedItem = _projectTeamRepository.GetProjectTeamById(id) ?? new ProjectTeam(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<ProjectTeam> ptVM)
        {
            if (ModelState.IsValid)
            {
                _projectTeamRepository.UpdateProjectTeam(ptVM.SelectedItem);
                return View("ProjectTeamEditor", ptVM);
            }
            return View("ProjectTeamEditor", ptVM);
        }

        public IActionResult Details(long id)
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { SelectedItem = _projectTeamRepository.GetProjectTeamById(id) ?? new ProjectTeam(), Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("ProjectTeamEditor", new SimpleEditFormVM<ProjectTeam>() { SelectedItem = new ProjectTeam(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<ProjectTeam> ptVM)
        {
            if (ModelState.IsValid)
            {
                _projectTeamRepository.CreateProjectTeam(ptVM.SelectedItem);
                ptVM.Mode = FormMode.Edit;
                return View("ProjectTeamEditor", ptVM);
            }
            return View("ProjectTeamEditor", ptVM);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _projectTeamRepository.DeleteProjectTeam(id);
            return RedirectToAction("ProjectTeamList");
        }
    }
}

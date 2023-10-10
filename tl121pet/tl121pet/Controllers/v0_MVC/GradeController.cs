using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize]
    public class GradeController : Controller
    {
        private IGradeService _gradeService;
        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        public async Task<IActionResult> GradeList()
        {
            return View("GradeList", await _gradeService.GetAllGradesAsync());
        }

        public async Task<IActionResult> Edit(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = await _gradeService.GetGradeByIdAsync(id),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                await _gradeService.UpdateGradeAsync(gradeVM.SelectedItem);
                return View("GradeEditor", gradeVM);
            }
            return View("GradeEditor", gradeVM);
        }

        public async Task<IActionResult> Details(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = await _gradeService.GetGradeByIdAsync(id),
                Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = new Grade(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                await _gradeService.CreateGradeAsync(gradeVM.SelectedItem);
                gradeVM.Mode = FormMode.Edit;
                return View("GradeEditor", gradeVM);
            }
            gradeVM.Mode = FormMode.Create;
            return View("GradeEditor", gradeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _gradeService.DeleteGradeAsync(id);
            return RedirectToAction("GradeList");
        }
    }
}

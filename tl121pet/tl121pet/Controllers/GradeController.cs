using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private IPersonService _personService;
        public GradeController(IPersonService personService)
        { 
            _personService = personService;
        }

        public async Task<IActionResult> GradeList()
        {
            return View("GradeList", await _personService.GetAllGradesAsync());
        }

        public async Task<IActionResult> Edit(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = await _personService.GetGradeByIdAsync(id),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                await _personService.UpdateGradeAsync(gradeVM.SelectedItem);
                return View("GradeEditor", gradeVM);
            }
            return View("GradeEditor", gradeVM);
        }

        public async Task<IActionResult> Details(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = await _personService.GetGradeByIdAsync(id),
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
                await _personService.CreateGradeAsync(gradeVM.SelectedItem);
                gradeVM.Mode = FormMode.Edit;
                return View("GradeEditor", gradeVM);
            }
            gradeVM.Mode = FormMode.Create;
            return View("GradeEditor", gradeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _personService.DeleteGradeAsync(id);
            return RedirectToAction("GradeList");
        }
    }
}

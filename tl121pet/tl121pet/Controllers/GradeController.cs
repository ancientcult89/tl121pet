using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private IGradeRepository _gradeRepository;
        private DataContext _dataContext;
        public GradeController(IGradeRepository gradeRepository, DataContext dataContext)
        { 
            _gradeRepository = gradeRepository;
            _dataContext = dataContext;
        }

        public async Task<IActionResult> GradeList()
        {
            return View("GradeList", await _gradeRepository.GetAllGradesAsync());
        }

        public IActionResult Edit(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = _dataContext.Grades.Find(id) ?? new Grade(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                await _gradeRepository.UpdateGradeAsync(gradeVM.SelectedItem);
                return View("GradeEditor", gradeVM);
            }
            return View("GradeEditor", gradeVM);
        }

        public IActionResult Details(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { 
                SelectedItem = _dataContext.Grades.Find(id) ?? new Grade(),
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
                await _gradeRepository.CreateGradeAsync(gradeVM.SelectedItem);
                gradeVM.Mode = FormMode.Edit;
                return View("GradeEditor", gradeVM);
            }
            gradeVM.Mode = FormMode.Create;
            return View("GradeEditor", gradeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _gradeRepository.DeleteGradeAsync(id);
            return RedirectToAction("GradeList");
        }
    }
}

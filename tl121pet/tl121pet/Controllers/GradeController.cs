using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    //[Authorize]
    public class GradeController : Controller
    {
        private IGradeRepository _gradeRepository;
        private DataContext _dataContext;
        private IAuthService _authService;
        public GradeController(IGradeRepository gradeRepository, DataContext dataContext, IAuthService authService)
        { 
            _gradeRepository = gradeRepository;
            _dataContext = dataContext;
            _authService = authService;
        }

        public IActionResult GradeList()
        {
            return View("GradeList", _gradeRepository.GetAllGrades());
        }

        public IActionResult Edit(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { SelectedItem = _dataContext.Grades.Find(id) ?? new Grade(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                _gradeRepository.UpdateGrade(gradeVM.SelectedItem);
                return View("GradeEditor", gradeVM);
            }
            return View("GradeEditor", gradeVM);
        }

        public IActionResult Details(long id)
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { SelectedItem = _dataContext.Grades.Find(id) ?? new Grade(), Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("GradeEditor", new SimpleEditFormVM<Grade>() { SelectedItem = new Grade(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<Grade> gradeVM)
        {
            if (ModelState.IsValid)
            {
                _gradeRepository.CreateGrade(gradeVM.SelectedItem);
                gradeVM.Mode = FormMode.Edit;
                return View("GradeEditor", gradeVM);
            }
            return View("GradeEditor", gradeVM);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _gradeRepository.DeleteGrade(id);
            return RedirectToAction("GradeList");
        }
    }
}

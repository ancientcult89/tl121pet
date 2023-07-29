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
    public class MeetingTypeController : Controller
    {
        private IMeetingRepository _meetingRepository;
        //TODO: избавиться от зависимости слоя данных в контроллере
        private DataContext _dataContext;
        public MeetingTypeController(DataContext dataContext, IMeetingRepository meetingRepository)
        {

            _dataContext = dataContext;
            _meetingRepository = meetingRepository;
        }

        #region MeetingTypes
        public IActionResult MeetingTypeList()
        {
            return View("MeetingTypeList", _dataContext.MeetingTypes.ToList());
        }

        public IActionResult Edit(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { 
                SelectedItem = _dataContext.MeetingTypes.Find(id) ?? new MeetingType(), 
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                await _meetingRepository.UpdateMeetingTypeAsync(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingTypeList");
            }
            meetingTypeVM.Mode = FormMode.Create;
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        public IActionResult Details(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { 
                SelectedItem = _dataContext.MeetingTypes.Find(id) ?? new MeetingType(),
                Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { 
                SelectedItem = new MeetingType(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                await _meetingRepository.CreateMeetingTypeAsync(meetingTypeVM.SelectedItem);
                meetingTypeVM.Mode = FormMode.Edit;
                return View("MeetingTypeEditor", meetingTypeVM);
            }
            meetingTypeVM.Mode = FormMode.Create;
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _meetingRepository.DeleteMeetingTypeAsync(id);
            return RedirectToAction("MeetingTypeList");
        }
        #endregion MeetingType
    }
}

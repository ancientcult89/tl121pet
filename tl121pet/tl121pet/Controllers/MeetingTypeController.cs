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
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = _dataContext.MeetingTypes.Find(id) ?? new MeetingType(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeetingType(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingTypeList");
            }
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        public IActionResult Details(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = _dataContext.MeetingTypes.Find(id) ?? new MeetingType(), Mode = FormMode.Details });
        }

        public IActionResult Create()
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = new MeetingType(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.CreateMeetingType(meetingTypeVM.SelectedItem);
                meetingTypeVM.Mode = FormMode.Create;
                return View("MeetingTypeEditor", meetingTypeVM);
            }
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _meetingRepository.DeleteMeetingType(id);
            return RedirectToAction("MeetingTypeList");
        }
        #endregion MeetingType
    }
}

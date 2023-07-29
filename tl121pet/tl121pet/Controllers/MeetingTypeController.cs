using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class MeetingTypeController : Controller
    {
        private IMeetingService _meetingService;
        public MeetingTypeController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        #region MeetingTypes
        public IActionResult MeetingTypeList()
        {
            return View("MeetingTypeList", _meetingService.GetAllMeetingTypesAsync());
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { 
                SelectedItem = await _meetingService.GetMeetingTypeByIdAsync(id), 
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                await _meetingService.UpdateMeetingTypeAsync(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingTypeList");
            }
            meetingTypeVM.Mode = FormMode.Create;
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { 
                SelectedItem = await _meetingService.GetMeetingTypeByIdAsync(id),
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
                await _meetingService.CreateMeetingTypeAsync(meetingTypeVM.SelectedItem);
                meetingTypeVM.Mode = FormMode.Edit;
                return View("MeetingTypeEditor", meetingTypeVM);
            }
            meetingTypeVM.Mode = FormMode.Create;
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _meetingService.DeleteMeetingTypeAsync(id);
            return RedirectToAction("MeetingTypeList");
        }
        #endregion MeetingType
    }
}

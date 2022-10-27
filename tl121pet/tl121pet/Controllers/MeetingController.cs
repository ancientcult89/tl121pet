using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class MeetingController : Controller
    {
        private IMeetingRepository _meetingRepository;
        private DataContext _dataContext;
        public MeetingController(DataContext dataContext, IMeetingRepository meetingRepository)
        {

            _dataContext = dataContext;
            _meetingRepository = meetingRepository;
        }
        public IActionResult Index()
        {
            return View("MeetingList", _dataContext.Meeting.Include(p => p.MeetingNotes).Include(p => p.Person));
        }

        public IActionResult MeetingTypeList()
        { 
            return View("MeetingTypeList", _dataContext.MeetingType.ToList());
        }

        public IActionResult MeetingTypeEdit(int id)
        {
            return View("MeetingTypeEditor", new BaseVM<MeetingType>() { SelectedItem = _dataContext.MeetingType.Find(id) ?? new MeetingType(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult MeetingTypeEdit([FromForm] BaseVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeetingType(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingList");
            }
            return View("PersonEditor", meetingTypeVM);
        }

        public IActionResult MeetingTypeDetails(int id)
        {
            return View("MeetingTypeEditor", new BaseVM<MeetingType>() { SelectedItem = _dataContext.MeetingType.Find(id) ?? new MeetingType(), Mode = FormMode.Details });
        }

        public IActionResult MeetingTypeCreate()
        {
            return View("MeetingTypeEditor", new BaseVM<MeetingType>() { SelectedItem = new MeetingType(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult MeetingTypeCreate([FromForm] BaseVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeetingType(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingTypeList");
            }
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        [HttpPost]
        public IActionResult DeleteMeetingType(int id)
        {
            _meetingRepository.DeleteMeetingType(id);
            return RedirectToAction("MeetingTypeList");
        }
    }
}

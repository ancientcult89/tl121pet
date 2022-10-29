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
        #region Meeting
        public IActionResult MeetingList()
        {
            return View("MeetingList", _dataContext.Meeting
                .Include(p => p.MeetingNotes)
                .Include(p => p.Person)
                .Include(p => p.MeetingType).ToList());
        }
        public IActionResult MeetingDetails(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = _dataContext.Meeting.Find(id) ?? new Meeting(), Mode = FormMode.Details });
        }

        public IActionResult MeetingCreate()
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = new Meeting(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult MeetingCreate([FromForm] MeetingEditFormVM meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.CreateMeeting(meetingVM.SelectedItem);
                return RedirectToAction("MeetingList");
            }
            return View("MeetingEditor", meetingVM);
        }

        public IActionResult MeetingProcess(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meeting.Find(id) ?? new Meeting()
                , Mode = FormMode.Process 
                , MeetingNotes = _meetingRepository.GetMeetingNotes(id)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(id)
            });
        }
        [HttpPost]
        public IActionResult MeetingProcess([FromForm] MeetingEditFormVM meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                return RedirectToAction("MeetingList");
            }
            return View("MeetingEditor", meetingVM);
        }
        public IActionResult MeetingEdit(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = _dataContext.Meeting.Find(id) ?? new Meeting(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult MeetingEdit([FromForm] MeetingEditFormVM meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                return RedirectToAction("MeetingList");
            }
            return View("MeetingEditor", meetingVM);
        }

        [HttpPost]
        public IActionResult DeleteMeeting(Guid id)
        {
            _meetingRepository.DeleteMeeting(id);
            return RedirectToAction("MeetingList");
        }
        #endregion Meeting

        #region MeetingTypes
        public IActionResult MeetingTypeList()
        { 
            return View("MeetingTypeList", _dataContext.MeetingType.ToList());
        }

        public IActionResult MeetingTypeEdit(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = _dataContext.MeetingType.Find(id) ?? new MeetingType(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult MeetingTypeEdit([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeetingType(meetingTypeVM.SelectedItem);
                return RedirectToAction("MeetingTypeList");
            }
            return View("MeetingTypeEditor", meetingTypeVM);
        }

        public IActionResult MeetingTypeDetails(int id)
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = _dataContext.MeetingType.Find(id) ?? new MeetingType(), Mode = FormMode.Details });
        }

        public IActionResult MeetingTypeCreate()
        {
            return View("MeetingTypeEditor", new SimpleEditFormVM<MeetingType>() { SelectedItem = new MeetingType(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult MeetingTypeCreate([FromForm] SimpleEditFormVM<MeetingType> meetingTypeVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.CreateMeetingType(meetingTypeVM.SelectedItem);
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
        #endregion MeetingType

        #region MeetingNotes
        [HttpPost]
        public IActionResult AddNote([FromForm] MeetingEditFormVM vm)
        {
            if (ModelState.IsValid)
            { 
                _meetingRepository.AddNote(vm.SelectedItem.MeetingId, vm.NewNote);                
            }
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meeting.Find(vm.SelectedItem.MeetingId) ?? new Meeting()
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(vm.SelectedItem.MeetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(vm.SelectedItem.MeetingId)
            });
        }

        public IActionResult DeleteNote(Guid goalId, Guid meetingId)
        { 
            _meetingRepository.DeleteNote(goalId);
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meeting.Find(meetingId) ?? new Meeting()
                , Mode = FormMode.Process 
                , MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
            });
        }
        #endregion MeetingNotes

        #region MeetingGoals
        [HttpPost]
        public IActionResult AddGoal([FromForm] MeetingEditFormVM vm)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.AddGoal(vm.SelectedItem.MeetingId, vm.NewGoal);
            }
            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = _dataContext.Meeting.Find(vm.SelectedItem.MeetingId) ?? new Meeting()
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(vm.SelectedItem.MeetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(vm.SelectedItem.MeetingId)
            });
        }

        public IActionResult DeleteGoal(Guid goalId, Guid meetingId)
        {
            _meetingRepository.DeleteGoal(goalId);
            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = _dataContext.Meeting.Find(meetingId) ?? new Meeting()
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
            });
        }
        #endregion MeetingGoals
    }
}

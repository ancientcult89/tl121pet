using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;
using tl121pet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace tl121pet.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private IMeetingRepository _meetingRepository;
        private readonly IMeetingService _meetingService;
        private DataContext _dataContext;
        private IOneToOneService _oneToOneService;
        public MeetingController(DataContext dataContext, 
            IMeetingRepository meetingRepository, 
            IMeetingService meetingService,
            IOneToOneService oneToOneService)
        {
            _dataContext = dataContext;
            _meetingRepository = meetingRepository;
            _meetingService = meetingService;
            _oneToOneService = oneToOneService;
        }
        #region Meeting
        public IActionResult MeetingList(long? personId = null)
        {
            return View("MeetingList", _meetingService.GetMeetings(personId));
        }
        public IActionResult Details(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meetings.Find(id) ?? new Meeting(), 
                Mode = FormMode.Details,
                MeetingNotes = _meetingRepository.GetMeetingNotes(id),
                MeetingGoals = _meetingRepository.GetMeetingGoals(id)
            });
        }

        public IActionResult FollowUp(Guid meetingId, long personId, FormMode mode)
        {            
            return View("FollowUp", new FollowUpVM() { 
                FollowUpMessage = _oneToOneService.GenerateFollowUp(meetingId, personId), 
                MeetingId = meetingId, 
                Mode = mode, 
                PersonId = personId 
            });
        }

        [HttpPost]
        public IActionResult FollowUp(Guid meetingId, FormMode mode, long personId)
        {
            _oneToOneService.SendFollowUp(meetingId, personId);
            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = _dataContext.Meetings.Find(meetingId) ?? new Meeting(),
                Mode = mode,
                MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId),
                MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
            });
        }

        public IActionResult Create()
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = new Meeting(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] MeetingEditFormVM meetingVM)
        {
            //if (ModelState.IsValid)
            //{
                _meetingRepository.CreateMeeting(meetingVM.SelectedItem);
                meetingVM.Mode = FormMode.Edit;
                return View("MeetingEditor", meetingVM);
            //}
            //return View("MeetingEditor", meetingVM);
        }

        public IActionResult Process(Guid id)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(id) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);
            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = currMeeting
                ,
                Mode = FormMode.Process
                ,
                MeetingNotes = _meetingRepository.GetMeetingNotes(id)
                ,
                MeetingGoals = _meetingRepository.GetMeetingGoals(id)
                ,
                PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }

        private string GetPreviousNotesAnfGoals(Meeting currMeeting)
        {
            string prevNotesAndGoals = "";
            Guid? previousMeetingGuid = _meetingRepository.GetPreviousMeetingId(currMeeting.MeetingId, currMeeting.PersonId);
            if (previousMeetingGuid != null)
            {
                prevNotesAndGoals = _oneToOneService.GetMeetingNoteAndGoals((Guid)previousMeetingGuid);
            }

            return prevNotesAndGoals;
        }

        [HttpPost]
        public IActionResult Process([FromForm] MeetingEditFormVM meetingVM)
        {
            //if (ModelState.IsValid)
            //{
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                meetingVM.Mode = FormMode.Process;
                meetingVM.MeetingNotes = _meetingRepository.GetMeetingNotes(meetingVM.SelectedItem.MeetingId);
                meetingVM.MeetingGoals = _meetingRepository.GetMeetingGoals(meetingVM.SelectedItem.MeetingId);
                return View("MeetingEditor", meetingVM);
            //}
            //return View("MeetingEditor", meetingVM);
        }
        public IActionResult Edit(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = _dataContext.Meetings.Find(id) ?? new Meeting(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] MeetingEditFormVM meetingVM)
        {
            //if (ModelState.IsValid)
            //{
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                meetingVM.MeetingNotes = _meetingRepository.GetMeetingNotes(meetingVM.SelectedItem.MeetingId);
                meetingVM.MeetingGoals = _meetingRepository.GetMeetingGoals(meetingVM.SelectedItem.MeetingId);
                return View("MeetingEditor", meetingVM);
            //}
            //return View("MeetingEditor", meetingVM);
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            _meetingRepository.DeleteMeeting(id);
            return RedirectToAction("MeetingList");
        }
        #endregion Meeting

        #region MeetingNotes
        [HttpPost]
        public IActionResult AddNote([FromForm] MeetingEditFormVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem.MeetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.AddNote(vm.SelectedItem.MeetingId, vm.NewNote, vm.NewNoteFeedbackRequires);
            
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = currMeeting
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(vm.SelectedItem.MeetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(vm.SelectedItem.MeetingId)
                , PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }

        [HttpPost]
        public IActionResult UpdateNote(bool FeedbackRequired, string MeetingNoteContent, Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.UpdateNote(noteId, MeetingNoteContent, FeedbackRequired);

            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = currMeeting,
                Mode = FormMode.Process,
                MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId),
                MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId),
                PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }

        public IActionResult DeleteNote(Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.DeleteNote(noteId);
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = currMeeting
                , Mode = FormMode.Process 
                , MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
                , PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }
        #endregion MeetingNotes

        #region MeetingGoals
        [HttpPost]
        public IActionResult AddGoal([FromForm] MeetingEditFormVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem.MeetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.AddGoal(vm.SelectedItem.MeetingId, vm.NewGoal);

            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = currMeeting
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(vm.SelectedItem.MeetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(vm.SelectedItem.MeetingId)
                , PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }

        [HttpPost]
        public IActionResult UpdateGoal(string MeetingGoalDescription, Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.UpdateGoal(goalId, MeetingGoalDescription);

            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = _dataContext.Meetings.Find(meetingId) ?? new Meeting(),
                Mode = FormMode.Process,
                MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId),
                MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId),
                PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }

        public IActionResult DeleteGoal(Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = GetPreviousNotesAnfGoals(currMeeting);

            _meetingRepository.DeleteGoal(goalId);

            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = currMeeting
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
                , PreviousMeetingNotesAndGoals = prevNotesAndGoals
            });
        }
        #endregion MeetingGoals
    }
}

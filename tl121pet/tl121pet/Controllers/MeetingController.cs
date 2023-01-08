using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;
using tl121pet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;

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
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = AutomapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(id)) ?? new MeetingDTO(), 
                Mode = FormMode.Details
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
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(meetingId)) ?? new MeetingDTO(),
                Mode = mode
            });
        }

        public IActionResult Create()
        {
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { SelectedItem = new MeetingDTO(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.CreateMeeting(AutomapperMini.MeetingDtoToEntity(meetingVM.SelectedItem));
                meetingVM.Mode = FormMode.Edit;
                return View("MeetingEditor", meetingVM);
            }
            meetingVM.Mode = FormMode.Create;
            return View("MeetingEditor", meetingVM);
        }

        public IActionResult Process(Guid id)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(id) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = _oneToOneService.GetPreviousMeetingNoteAndGoals(currMeeting.MeetingId, currMeeting.PersonId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult Process([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting editedMeeting = _dataContext.Meetings.Find(meetingVM.SelectedItem.MeetingId);
                editedMeeting.MeetingPlanDate = meetingVM.SelectedItem.MeetingPlanDate;
                editedMeeting.MeetingDate = meetingVM.SelectedItem.MeetingDate;
                editedMeeting.MeetingTypeId = meetingVM.SelectedItem.MeetingTypeId;
                editedMeeting.FollowUpIsSended = meetingVM.SelectedItem.FollowUpIsSended;
                editedMeeting.PersonId = meetingVM.SelectedItem.PersonId;

                _meetingRepository.UpdateMeeting(editedMeeting);
                meetingVM.Mode = FormMode.Process;
                return View("MeetingEditor", meetingVM);
        }
            return View("MeetingEditor", meetingVM);
    }
        public IActionResult Edit(Guid id)
        {
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = AutomapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(id)) ?? new MeetingDTO()
                , Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting editedMeeting = _dataContext.Meetings.Find(meetingVM.SelectedItem.MeetingId);
                editedMeeting.MeetingPlanDate = meetingVM.SelectedItem.MeetingPlanDate;
                editedMeeting.MeetingDate = meetingVM.SelectedItem.MeetingDate;
                editedMeeting.MeetingTypeId = meetingVM.SelectedItem.MeetingTypeId;
                editedMeeting.FollowUpIsSended = meetingVM.SelectedItem.FollowUpIsSended;
                editedMeeting.PersonId = meetingVM.SelectedItem.PersonId;

                _meetingRepository.UpdateMeeting(editedMeeting);

                return View("MeetingEditor", meetingVM);
            }
            return View("MeetingEditor", meetingVM);
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
        public IActionResult AddNote([FromForm] NoteEditListVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem) ?? new Meeting();

            _meetingRepository.AddNote(vm.SelectedItem, vm.NewNote, vm.NewNoteFeedbackRequires);
            
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting), 
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult UpdateNote(bool FeedbackRequired, string MeetingNoteContent, Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = _oneToOneService.GetPreviousMeetingNoteAndGoals(currMeeting.MeetingId, currMeeting.PersonId);

            _meetingRepository.UpdateNote(noteId, MeetingNoteContent, FeedbackRequired);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        public IActionResult DeleteNote(Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = _oneToOneService.GetPreviousMeetingNoteAndGoals(currMeeting.MeetingId, currMeeting.PersonId);

            _meetingRepository.DeleteNote(noteId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting)
                , Mode = FormMode.Process
            });
        }
        #endregion MeetingNotes

        #region MeetingGoals
        [HttpPost]
        public IActionResult AddGoal([FromForm] GoalEditListVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = _oneToOneService.GetPreviousMeetingNoteAndGoals(currMeeting.MeetingId, currMeeting.PersonId);

            _meetingRepository.AddGoal(vm.SelectedItem, vm.NewGoal);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting)
                , Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult UpdateGoal(string MeetingGoalDescription, Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();
            string prevNotesAndGoals = "";
            prevNotesAndGoals = _oneToOneService.GetPreviousMeetingNoteAndGoals(currMeeting.MeetingId, currMeeting.PersonId);

            _meetingRepository.UpdateGoal(goalId, MeetingGoalDescription);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(meetingId)) ?? new MeetingDTO(),
                Mode = FormMode.Process
            });
        }

        public IActionResult DeleteGoal(Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();

            _meetingRepository.DeleteGoal(goalId);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = AutomapperMini.MeetingEntityToDto(currMeeting)
                , Mode = FormMode.Process
            });
        }
        #endregion MeetingGoals
    }
}

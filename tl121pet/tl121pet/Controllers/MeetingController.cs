﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IAutomapperMini _automapperMini;
        public MeetingController(DataContext dataContext, 
            IMeetingRepository meetingRepository, 
            IMeetingService meetingService,
            IOneToOneService oneToOneService,
            IAutomapperMini automapperMini)
        {
            _dataContext = dataContext;
            _meetingRepository = meetingRepository;
            _meetingService = meetingService;
            _oneToOneService = oneToOneService;
            _automapperMini = automapperMini;
        }
        #region Meeting
        public async Task<IActionResult> MeetingList(long? personId = null)
        {
            return View("MeetingList", await _meetingService.GetMeetingsAsync(personId));
        }
        public IActionResult Details(Guid id)
        {
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(id)) ?? new MeetingDTO(), 
                Mode = FormMode.Details
            });
        }

        public async Task<IActionResult> FollowUp(Guid meetingId, long personId, FormMode mode)
        {            
            return View("FollowUp", new FollowUpVM() { 
                FollowUpMessage = await _oneToOneService.GenerateFollowUpAsync(meetingId, personId), 
                MeetingId = meetingId, 
                Mode = mode, 
                PersonId = personId 
            });
        }

        [HttpPost]
        public async Task<IActionResult> FollowUp(Guid meetingId, FormMode mode, long personId)
        {
            await _oneToOneService.SendFollowUpAsync(meetingId, personId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(meetingId)) ?? new MeetingDTO(),
                Mode = mode
            });
        }

        public IActionResult Create()
        {
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = new MeetingDTO(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting m = _meetingRepository.CreateMeeting(_automapperMini.MeetingDtoToEntity(meetingVM.SelectedItem));
                meetingVM.Mode = FormMode.Edit;
                meetingVM.SelectedItem = _automapperMini.MeetingEntityToDto(m);
                return View("MeetingEditor", meetingVM);
            }
            meetingVM.Mode = FormMode.Create;
            return View("MeetingEditor", meetingVM);
        }

        public IActionResult Process(Guid id)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(id) ?? new Meeting();

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult Process([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                meetingVM.Mode = FormMode.Process;
                return View("MeetingEditor", meetingVM);
            }
            return View("MeetingEditor", meetingVM);
        }
        public IActionResult Edit(Guid id)
        {
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(id)) ?? new MeetingDTO(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
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
            if(ModelState.IsValid)
                _meetingRepository.AddNote(vm.SelectedItem, vm.NewNote, vm.NewNoteFeedbackRequires);
            
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting), 
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult UpdateNote(bool FeedbackRequired, string MeetingNoteContent, Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();

            _meetingRepository.UpdateNote(noteId, MeetingNoteContent, FeedbackRequired);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        public IActionResult DeleteNote(Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();

            _meetingRepository.DeleteNote(noteId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }
        #endregion MeetingNotes

        #region MeetingGoals
        [HttpPost]
        public IActionResult AddGoal([FromForm] GoalEditListVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem) ?? new Meeting();

            if (ModelState.IsValid)
                _meetingRepository.AddGoal(vm.SelectedItem, vm.NewGoal);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public IActionResult UpdateGoal(string MeetingGoalDescription, Guid goalId, Guid meetingId)
        {
            _meetingRepository.UpdateGoal(goalId, MeetingGoalDescription);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(_dataContext.Meetings.Find(meetingId)) ?? new MeetingDTO(),
                Mode = FormMode.Process
            });
        }

        public IActionResult DeleteGoal(Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(meetingId) ?? new Meeting();

            _meetingRepository.DeleteGoal(goalId);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }
        #endregion MeetingGoals
    }
}

using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;
using tl121pet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly IMeetingService _meetingService;
        //TODO: избавиться от зависимости слоя данных в контроллере
        private DataContext _dataContext;
        private IOneToOneService _oneToOneService;
        private readonly IAutomapperMini _automapperMini;
        public MeetingController(DataContext dataContext, 
            IMeetingService meetingService,
            IOneToOneService oneToOneService,
            IAutomapperMini automapperMini)
        {
            _dataContext = dataContext;
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
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting m = await _meetingService.CreateMeetingAsync(_automapperMini.MeetingDtoToEntity(meetingVM.SelectedItem));
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
        public async Task<IActionResult> Process([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting meeting = _automapperMini.MeetingDtoToEntity(meetingVM.SelectedItem);
                await _meetingService.UpdateMeetingAsync(meeting);
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
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<MeetingDTO> meetingVM)
        {
            if (ModelState.IsValid)
            {
                Meeting meting = _automapperMini.MeetingDtoToEntity(meetingVM.SelectedItem);
                await _meetingService.UpdateMeetingAsync(meting);
                return View("MeetingEditor", meetingVM);
            }
            return View("MeetingEditor", meetingVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _meetingService.DeleteMeetingAsync(id);
            return RedirectToAction("MeetingList");
        }
        #endregion Meeting

        #region MeetingNotes
        [HttpPost]
        public async Task<IActionResult> AddNote([FromForm] NoteEditListVM vm)
        {
            Meeting currMeeting = _dataContext.Meetings.Find(vm.SelectedItem) ?? new Meeting();
            if(ModelState.IsValid)
                await _meetingService.AddNoteAsync(vm.SelectedItem, vm.NewNote, vm.NewNoteFeedbackRequires);
            
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting), 
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNote(bool FeedbackRequired, string MeetingNoteContent, Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = await _dataContext.Meetings.FindAsync(meetingId) ?? new Meeting();

            await _meetingService.UpdateNoteAsync(noteId, MeetingNoteContent, FeedbackRequired);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        public async Task<IActionResult> DeleteNote(Guid noteId, Guid meetingId)
        {
            Meeting currMeeting = await _dataContext.Meetings.FindAsync(meetingId) ?? new Meeting();

            await _meetingService.DeleteNoteAsync(noteId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>() { 
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }
        #endregion MeetingNotes

        #region MeetingGoals
        [HttpPost]
        public async Task<IActionResult> AddGoal([FromForm] GoalEditListVM vm)
        {
            Meeting currMeeting = await _dataContext.Meetings.FindAsync(vm.SelectedItem) ?? new Meeting();

            if (ModelState.IsValid)
                await _meetingService.AddGoalAsync(vm.SelectedItem, vm.NewGoal);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGoal(string MeetingGoalDescription, Guid goalId, Guid meetingId)
        {
            await _meetingService.UpdateGoalTaskAsync(goalId, MeetingGoalDescription);
            Meeting currentMeeting = await _dataContext.Meetings.FindAsync(meetingId);
            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currentMeeting) ?? new MeetingDTO(),
                Mode = FormMode.Process
            });
        }

        public async Task<IActionResult> DeleteGoal(Guid goalId, Guid meetingId)
        {
            Meeting currMeeting = await _dataContext.Meetings.FindAsync(meetingId) ?? new Meeting();

            await _meetingService.DeleteGoalAsync(goalId);

            return View("MeetingEditor", new SimpleEditFormVM<MeetingDTO>()
            {
                SelectedItem = _automapperMini.MeetingEntityToDto(currMeeting),
                Mode = FormMode.Process
            });
        }
        #endregion MeetingGoals
    }
}

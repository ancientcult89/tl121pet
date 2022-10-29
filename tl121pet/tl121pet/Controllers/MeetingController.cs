﻿using Microsoft.AspNetCore.Mvc;
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
            return View("MeetingList", _dataContext.Meetings
                .Include(p => p.MeetingNotes)
                .Include(p => p.Person)
                .Include(p => p.MeetingType).ToList());
        }
        public IActionResult Details(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { SelectedItem = _dataContext.Meetings.Find(id) ?? new Meeting(), Mode = FormMode.Details });
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
                return RedirectToAction("MeetingList");
            //}
            //return View("MeetingEditor", meetingVM);
        }

        public IActionResult Process(Guid id)
        {
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meetings.Find(id) ?? new Meeting()
                , Mode = FormMode.Process 
                , MeetingNotes = _meetingRepository.GetMeetingNotes(id)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(id)
            });
        }
        [HttpPost]
        public IActionResult Process([FromForm] MeetingEditFormVM meetingVM)
        {
            //if (ModelState.IsValid)
            //{
                _meetingRepository.UpdateMeeting(meetingVM.SelectedItem);
                return RedirectToAction("MeetingList");
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
                return RedirectToAction("MeetingList");
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

            _meetingRepository.AddNote(vm.SelectedItem.MeetingId, vm.NewNote);
            
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meetings.Find(vm.SelectedItem.MeetingId) ?? new Meeting()
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(vm.SelectedItem.MeetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(vm.SelectedItem.MeetingId)
            });
        }

        public IActionResult DeleteNote(Guid goalId, Guid meetingId)
        { 
            _meetingRepository.DeleteNote(goalId);
            return View("MeetingEditor", new MeetingEditFormVM() { 
                SelectedItem = _dataContext.Meetings.Find(meetingId) ?? new Meeting()
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
            _meetingRepository.AddGoal(vm.SelectedItem.MeetingId, vm.NewGoal);

            return View("MeetingEditor", new MeetingEditFormVM()
            {
                SelectedItem = _dataContext.Meetings.Find(vm.SelectedItem.MeetingId) ?? new Meeting()
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
                SelectedItem = _dataContext.Meetings.Find(meetingId) ?? new Meeting()
                , Mode = FormMode.Process
                , MeetingNotes = _meetingRepository.GetMeetingNotes(meetingId)
                , MeetingGoals = _meetingRepository.GetMeetingGoals(meetingId)
            });
        }
        #endregion MeetingGoals
    }
}

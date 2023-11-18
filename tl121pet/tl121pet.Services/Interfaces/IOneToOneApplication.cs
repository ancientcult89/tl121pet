﻿using tl121pet.Entities.Aggregate;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneApplication
    {
        public Task<List<OneToOneDeadline>> GetDeadLinesAsync();
        public Task<string> GenerateFollowUpAsync(Guid meetingId, long personId);
        public Task SendFollowUpAsync(Guid meetingId, long personId);
        public Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId);
        public Task<List<Person>> GetPeopleFilteredByProjectsAsync();
        public Task<List<TaskDTO>> GetTaskListAsync(long? personId);
        public Task<List<Meeting>> GetMeetingsAsync(long? personId);
    }
}
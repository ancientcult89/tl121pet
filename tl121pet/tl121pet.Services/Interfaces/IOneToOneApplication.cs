using tl121pet.Entities.Aggregate;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IOneToOneApplication
    {
        public Task<List<OneToOneDeadline>> GetDeadLinesAsync();
        public Task<string> GenerateFollowUpAsync(Guid meetingId, long personId);
        public Task SendFollowUpAsync(Guid meetingId, long personId);
        public Task SendGreetingMailAsync(long personId);
        public Task ChangeLocaleAsync(int localeId);
        public Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId);
        public Task<List<Person>> GetPeopleFilteredByProjectsAsync();
        public Task<List<TaskDTO>> GetTaskListAsync(long? personId, Guid? currentMeetingId);
        public Task<MeetingPagedResponseDTO> GetPagedMeetingsAsync(MeetingPagedRequestDTO request);
        public Task<Meeting> CreateMeetingAsync(MeetingDTO m);
        public Task<Meeting> UpdateMeetingAsync(MeetingDTO m);
        public Task<Meeting> CreateMeetingByPersonIdAsync(long personId);
        public Task ArchivePersonAsync(long id);
        public Task RecoverPasswordAsync(RecoverPasswordRequestDTO recoverPasswordRequest);
    }
}

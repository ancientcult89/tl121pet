using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class MeetingNoteDtoExtension
    {
        public static MeetingNote ToEntity(this MeetingNoteDTO meetingNoteDTO)
        {
            MeetingNote note = new MeetingNote()
            {
                MeetingId = meetingNoteDTO.MeetingId,
                FeedbackRequired = meetingNoteDTO.FeedbackRequired,
                MeetingNoteContent = meetingNoteDTO.MeetingNoteContent,
            };
            if (meetingNoteDTO.MeetingNoteId != null)
                note.MeetingNoteId = (Guid)meetingNoteDTO.MeetingNoteId;

            return note;
        }
    }
}

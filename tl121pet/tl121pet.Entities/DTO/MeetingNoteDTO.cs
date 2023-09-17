﻿namespace tl121pet.Entities.DTO
{
    public class MeetingNoteDTO
    {
        public Guid? MeetingNoteId { get; set; }
        public string MeetingNoteContent { get; set; } = string.Empty;
        public Guid MeetingId { get; set; }
        public bool FeedbackRequired { get; set; } = false;
    }
}

using System;
using tl121pet.Entities.DTO;

namespace tl121pet.Tests.TestData
{
    public class MeetingDtoTestData
    {
        public static MeetingDTO GetSingleMeetingDto()
        {
            return new MeetingDTO()
            {
                MeetingId = new System.Guid("e5f1df56-4697-4e83-a8e9-9c12bb71804d"),
                MeetingPlanDate = new DateTime(2023, 01, 01),
                MeetingDate = new DateTime(2023, 01, 01),
                FollowUpIsSended = true,
                PersonId = 1
            };
        }
    }
}

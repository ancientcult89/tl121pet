using System;
using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;

namespace tl121pet.Tests.TestData
{
    public class MeetingTestData
    {
        public static Meeting GetSingleMeeting()
        {
            return new Meeting
            {
                MeetingId = new System.Guid("e5f1df56-4697-4e83-a8e9-9c12bb71804d"),
                MeetingPlanDate = new DateTime(2023, 01, 01),
                MeetingDate = new DateTime(2023, 01, 01),
                FollowUpIsSended = true,
                PersonId = 1
            };
        }

        public static IEnumerable<Meeting> GetTestMeetings()
        {
            return new List<Meeting>
            {
                new Meeting
                {
                    MeetingId = new System.Guid("e5f1df56-4697-4e83-a8e9-9c12bb71804d"),
                    MeetingPlanDate = new DateTime(2023, 01, 01),
                    MeetingDate = new DateTime(2023, 01, 01),
                    FollowUpIsSended = true,
                    PersonId = 1
                },
                new Meeting
                {
                    MeetingId = new System.Guid("13aeba16-dff5-4722-8753-0f3ddf787707"),
                    MeetingPlanDate = new DateTime(2023, 01, 02),
                    MeetingDate = new DateTime(2023, 01, 02),
                    FollowUpIsSended = false,
                    PersonId = 2
                },
            };
        }

        public static void FillTestDatabaseByPersons(DataContext dataContext)
        {
            IEnumerable<Meeting> meetings = (IEnumerable<Meeting>)GetTestMeetings();
            dataContext.Meetings.AddRange(meetings);
            dataContext.SaveChanges();
        }
    }
}

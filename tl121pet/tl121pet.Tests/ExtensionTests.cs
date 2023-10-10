using FluentAssertions;
using System;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Models;
using tl121pet.Tests.TestData;
using Xunit;

namespace tl121pet.Tests
{
    public class ExtensionTests
    {
        /// <summary>
        /// Метод проверяет что метод расширения класса Meeting возвращает корректную DTO
        /// </summary>
        [Fact]
        public void MeetingToDTO_ShouldReturnsCorrectDTO()
        {
            //Arrange
            Meeting meeting = MeetingTestData.GetSingleMeeting();
            MeetingDTO expect = new MeetingDTO()
            {
                MeetingId = new System.Guid("e5f1df56-4697-4e83-a8e9-9c12bb71804d"),
                MeetingPlanDate = new DateTime(2023, 01, 01),
                MeetingDate = new DateTime(2023, 01, 01),
                FollowUpIsSended = true,
                PersonId = 1
            };

            // Act
            var resultEntity = meeting.ToDto();

            // Assert
            resultEntity.Should().BeEquivalentTo(expect);
        }

        /// <summary>
        /// Метод проверяет что метод расширения класса Meeting возвращает корректную DTO
        /// </summary>
        [Fact]
        public void MeetingDtoToEntity_ShouldReturnsCorrectEntity()
        {
            //Arrange
            MeetingDTO meetingDto = MeetingDtoTestData.GetSingleMeetingDto();
            Meeting expect = new Meeting()
            {
                MeetingId = new System.Guid("e5f1df56-4697-4e83-a8e9-9c12bb71804d"),
                MeetingPlanDate = new DateTime(2023, 01, 01),
                MeetingDate = new DateTime(2023, 01, 01),
                FollowUpIsSended = true,
                PersonId = 1
            };

            // Act
            var resultEntity = meetingDto.ToEntity();

            // Assert
            resultEntity.Should().BeEquivalentTo(expect);
        }
    }
}
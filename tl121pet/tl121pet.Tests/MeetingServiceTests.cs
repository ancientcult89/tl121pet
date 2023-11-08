using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using Xunit;

namespace tl121pet.Tests
{
    public class MeetingServiceTests : IDisposable
    {
        private DataContext _dataContext;
        private IMeetingService _meetingService;
        public MeetingServiceTests()
        {
            var connectionString = "Server=host.docker.internal;Database=TLMeetingTest;Port=49153;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            _meetingService = new MeetingService(_dataContext);
        }
        public void Dispose()
        {
            _dataContext.Database.EnsureDeleted();
        }

        /// <summary>
        /// проверяем, что CreateMeetingAsync создаёт корректную встречу
        /// </summary>
        [Fact]
        public async void CreateMeetingAsync_ShouldCreateMeeting()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime createdAt = DateTime.Now;

            _dataContext.ProjectTeams.Add(sourseTeam);
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();
            Person createdPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(createdPerson);
            _dataContext.SaveChanges();

            //Act
            Meeting createdMeeting = new Meeting()
            {
                MeetingPlanDate = createdAt,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            await _meetingService.CreateMeetingAsync(createdMeeting);
            Meeting expectedMeeting = new Meeting()
            {
                MeetingPlanDate = createdAt,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
                MeetingId = createdMeeting.MeetingId,
            };

            //Assert
            createdMeeting.Should().BeEquivalentTo(expectedMeeting);
        }

        /// <summary>
        /// проверяем, что при попытке создать дублирующуюся встречу получим ошибку
        /// </summary>
        [Fact]
        public async void CreateDuplicatedMeeting_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime createdAt = DateTime.Now;

            _dataContext.ProjectTeams.Add(sourseTeam);
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();
            Person createdPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(createdPerson);
            _dataContext.SaveChanges();
            Meeting createdMeeting = new Meeting()
            {
                MeetingPlanDate = createdAt,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            await _meetingService.CreateMeetingAsync(createdMeeting);
            Meeting duplicatedMeeting = new Meeting()
            {
                MeetingPlanDate = createdAt,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            //Act
            var result = async () => await _meetingService.CreateMeetingAsync(duplicatedMeeting);

            //Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("The Meeting with this person is already planned");
        }
    }
}

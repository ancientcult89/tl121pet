using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
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

        #region Meeting
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
            await result.Should().ThrowAsync<LogicException>().WithMessage("The Meeting with this person on that date is already planned");
        }

        /// <summary>
        /// проверяем, что GetMeetingsByPersonAsync возвращает корректный список встреч по одному сотруднику
        /// </summary>
        [Fact]
        public async void GetMeetingsByPersonAsync_ShoudReturnCorrectSinglePersonMeetings()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(-1);

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
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting, createdMeeting2);
            _dataContext.SaveChanges();

            List<Person> personList = new List<Person>() { createdPerson };

            List<Meeting> expectedMeetingList = new List<Meeting>() {
                new Meeting()
                {
                    MeetingPlanDate = meetingDate,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting.MeetingId,
                },
                new Meeting()
                {
                    MeetingPlanDate = meetingDate2,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting2.MeetingId,
                },
            };

            //Act
            var result = await _meetingService.GetMeetingsByPersonAsync(personList);

            //Assert
            result.Should().BeEquivalentTo(expectedMeetingList);
        }

        /// <summary>
        /// проверяем, что GetMeetingsByPersonAsync возвращает корректный список встреч по нескольким сотрудникам
        /// </summary>
        [Fact]
        public async void GetMeetingsByPersonAsync_ShoudReturnCorrectSomePersonMeetings()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(-1);

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
            Person createdPerson2 = new Person()
            {
                Email = "1111@test1.com",
                FirstName = "Carl",
                LastName = "Urban",
                GradeId = testGrade.GradeId,
                ShortName = "Carlito",
                SurName = "Chupacabra",
                Grade = testGrade
            };
            _dataContext.People.AddRange(createdPerson, createdPerson2);
            _dataContext.SaveChanges();

            Meeting createdMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting3 = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson2.PersonId,
                Person = createdPerson2,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting4 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                PersonId = createdPerson2.PersonId,
                Person = createdPerson2,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting, createdMeeting2, createdMeeting3, createdMeeting4);
            _dataContext.SaveChanges();

            List<Person> personList = new List<Person>() { createdPerson, createdPerson2 };

            List<Meeting> expectedMeetingList = new List<Meeting>() {
                new Meeting()
                {
                    MeetingPlanDate = meetingDate,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting.MeetingId,
                },
                new Meeting()
                {
                    MeetingPlanDate = meetingDate2,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting2.MeetingId,
                },
                new Meeting()
                {
                    MeetingPlanDate = meetingDate,
                    PersonId = createdPerson2.PersonId,
                    Person = createdPerson2,
                    FollowUpIsSended = false,
                    MeetingId= createdMeeting3.MeetingId,
                },
                new Meeting()
                {
                    MeetingPlanDate = meetingDate2,
                    PersonId = createdPerson2.PersonId,
                    Person = createdPerson2,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting4.MeetingId
                }
            };

            //Act
            var result = await _meetingService.GetMeetingsByPersonAsync(personList);

            //Assert
            result.Should().BeEquivalentTo(expectedMeetingList);
        }

        /// <summary>
        /// проверяем, что GetMeetingsByPersonAsync возвращает корректный список встреч по одному сотруднику при наличии списка встреч по другим сотрудникам
        /// </summary>
        [Fact]
        public async void GetMeetingsByPersonAsync_ShoudReturnCorrectSinglePersonWithExistsOtherPersonMeetings()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(-1);

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
            Person createdPerson2 = new Person()
            {
                Email = "1111@test1.com",
                FirstName = "Carl",
                LastName = "Urban",
                GradeId = testGrade.GradeId,
                ShortName = "Carlito",
                SurName = "Chupacabra",
                Grade = testGrade
            };
            _dataContext.People.AddRange(createdPerson, createdPerson2);
            _dataContext.SaveChanges();

            Meeting createdMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting3 = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson2.PersonId,
                Person = createdPerson2,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting4 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                PersonId = createdPerson2.PersonId,
                Person = createdPerson2,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting, createdMeeting2, createdMeeting3, createdMeeting4);
            _dataContext.SaveChanges();

            List<Person> personList = new List<Person>() { createdPerson };

            List<Meeting> expectedMeetingList = new List<Meeting>() {
                new Meeting()
                {
                    MeetingPlanDate = meetingDate,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting.MeetingId,
                },
                new Meeting()
                {
                    MeetingPlanDate = meetingDate2,
                    PersonId = createdPerson.PersonId,
                    Person = createdPerson,
                    FollowUpIsSended = false,
                    MeetingId = createdMeeting2.MeetingId,
                },
            };

            //Act
            var result = await _meetingService.GetMeetingsByPersonAsync(personList);

            //Assert
            result.Should().BeEquivalentTo(expectedMeetingList);
        }

        /// <summary>
        /// проверяем, что при отсутсвии встреч мы получим пустую коллекцию, а не ошибку
        /// </summary>
        [Fact]
        public async void GetMeetingsByPersonAsync_ShouldReturnEmpryCollectionIfMeetingsNotExists()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };

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
            List<Person> personList = new List<Person>() { createdPerson };

            //Act
            var result = await _meetingService.GetMeetingsByPersonAsync(personList);

            //Assert
            result.Should().BeEmpty();
        }

        /// <summary>
        /// проверяем что GetMeetingByIdAsync возвращает корретную запись встречи
        /// </summary>
        [Fact]
        public async void GetMeetingByIdAsync_ShouldReturnCorrectMeeting()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;

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
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            Meeting expectedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
                MeetingId = createdMeeting.MeetingId,
            };

            //Act
            var result = await _meetingService.GetMeetingByIdMeetingAsync(createdMeeting.MeetingId);

            //Assert
            result.Should().BeEquivalentTo(expectedMeeting);
        }

        /// <summary>
        /// Проверяем, что при попытке получить несуществующую запись встречи получим ошибку
        /// </summary>
        [Fact]
        public async void GetNotExistsMeetingByIdAsync_ShouldThrowException()
        {
            //Arrange
            Guid notExistMeetingId = new Guid();

            //Act
            var result = async () => await _meetingService.GetMeetingByIdMeetingAsync(notExistMeetingId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Meeting not found");
        }

        /// <summary>
        /// проверяем, что UpdateMeetingAsync корректно обновляет запись
        /// </summary>
        [Fact]
        public async void UpdateMeetingAsync_ShouldUpdateMeeting()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;

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
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            Meeting updatedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = true,
                MeetingId = createdMeeting.MeetingId,
            };

            Meeting expectedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = true,
                MeetingId = createdMeeting.MeetingId,
            };

            //Act
            await _meetingService.UpdateMeetingAsync(updatedMeeting);

            //Assert
            updatedMeeting.Should().BeEquivalentTo(expectedMeeting);
        }

        /// <summary>
        /// Проверяем что при попытке обновить несуществующую запись получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistsMeeting_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;

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

            Meeting updatedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = true,
                MeetingId = new Guid(),
            };

            //Act
            var result = async () => await _meetingService.UpdateMeetingAsync(updatedMeeting);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Meeting not found");
        }

        /// <summary>
        /// проверяем, что при попытке обновить встречу с сотрудником на уже имеющуюся дату получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateMeetingByDuplicatingPlanDateOnPerson_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;
            DateTime plannedMeetingDate2 = DateTime.Now.AddDays(-1);

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = plannedMeetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting2, createdMeeting);
            _dataContext.SaveChanges();

            Meeting firstMeetingWithSecondPlannedDate = new Meeting()
            {
                MeetingPlanDate = plannedMeetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
                MeetingId = createdMeeting.MeetingId,
            };

            //Act
            var result = async () => await _meetingService.UpdateMeetingAsync(firstMeetingWithSecondPlannedDate);

            //Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("The Meeting with this person on that date is already planned");
        }

        /// <summary>
        /// проверяем что DeleteMeetingAsync удаляет запись встречи
        /// </summary>
        [Fact]
        public async void DeleteMeetingAsync_ShouldDeleteMeeting()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;
            DateTime plannedMeetingDate2 = DateTime.Now.AddDays(-1);

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();
            Guid meetingGuid = createdMeeting.MeetingId;

            //Act
            await _meetingService.DeleteMeetingAsync(meetingGuid);
            Meeting resultMeeting = _dataContext.Meetings.Find(meetingGuid);

            //Assert
            resultMeeting.Should().BeNull();
        }

        /// <summary>
        /// проверяем, что при попытке удалить несуществующую встречу получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsMeeting_ShouldThrowException()
        {
            //Arrange
            Guid notExistMeetingId = Guid.NewGuid();

            //Act
            var result = async () => await _meetingService.DeleteMeetingAsync(notExistMeetingId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Meeting not found");
        }
        #endregion Meeting

        #region MeetinNote
        /// <summary>
        /// проверяем что AddNoteAsync добавляет заметку
        /// </summary>
        [Fact]
        public async void AddNoteAsync_ShouldAddNote()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
            };

            //Act
            await _meetingService.AddNoteAsync(createdMeetingNote);

            MeetingNote expectedNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
                MeetingNoteId = createdMeetingNote.MeetingNoteId,
            };

            //Assert
            createdMeetingNote.Should().BeEquivalentTo(expectedNote);
        }

        /// <summary>
        /// проверяем, что при попытке добавить заметку на несуществующую встречу получим ошибку
        /// </summary>
        [Fact]
        public async void AddNoteToNotExistsMeeting_ShouldThrowException()
        {
            //Arrange
            Guid notExistsMeetingId = new Guid();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = notExistsMeetingId,
                MeetingNoteContent = "test",
            };

            //Act
            var result = async () => await _meetingService.AddNoteAsync(createdMeetingNote);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Meeting not found");
        }

        /// <summary>
        /// проверяем что UpdateNoteAsync обновляет заметку
        /// </summary>
        [Fact]
        public async void UpdateNoteAsync_ShouldUpdateNote()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
            };
            _dataContext.MeetingNotes.Add(createdMeetingNote);
            _dataContext.SaveChanges();

            //Act
            MeetingNote updatedNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test2",
                MeetingNoteId = createdMeetingNote.MeetingNoteId,
            };
            await _meetingService.UpdateNoteAsync(updatedNote);

            MeetingNote expectedNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test2",
                MeetingNoteId = createdMeetingNote.MeetingNoteId,
            };

            //Assert
            updatedNote.Should().BeEquivalentTo(expectedNote);
        }

        /// <summary>
        /// проверяем что при попытке обновить несуществующую заметку получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistNote_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;
            Guid notExistNoteId = new Guid();

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            MeetingNote updatedNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test2",
                MeetingNoteId = notExistNoteId,
            };

            //Act
            var result = async () => await _meetingService.UpdateNoteAsync(updatedNote);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Note not found");
        }

        /// <summary>
        /// проверяем, что DeleteNoteAsync удаляет заметку
        /// </summary>
        [Fact]
        public async void DeleteNoteAsync_ShouldDeleteNote()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
            };
            _dataContext.MeetingNotes.Add(createdMeetingNote);
            _dataContext.SaveChanges();

            //Act
            await _meetingService.DeleteNoteAsync(createdMeetingNote.MeetingNoteId);
            MeetingNote deletedNote = _dataContext.MeetingNotes.Find(createdMeetingNote.MeetingNoteId);

            //Assert
            deletedNote.Should().BeNull();
        }

        /// <summary>
        /// проверяем что при попытке удалить несуществующую заметку получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsNote_ShouldThrowException()
        {
            //Arrange
            Guid notExistNote = new Guid();

            //Act
            var result = async () => await _meetingService.DeleteNoteAsync(notExistNote);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Note not found");
        }

        /// <summary>
        /// проверяем что GetMeetingNotesAsync возвращает корректный список заметок
        /// </summary>
        [Fact]
        public async void GetMeetingNotesAsync_ShouldReturnCorrectNotes()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
            };
            MeetingNote createdMeetingNote2 = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test2",
            };
            _dataContext.MeetingNotes.AddRange(createdMeetingNote, createdMeetingNote2);
            _dataContext.SaveChanges();

            List<MeetingNote> expectedNotes = new List<MeetingNote>()
            {
                new MeetingNote
                {
                    FeedbackRequired = true,
                    MeetingId = createdMeeting.MeetingId,
                    MeetingNoteContent = "test",
                    MeetingNoteId = createdMeetingNote.MeetingNoteId,
                },
                new MeetingNote()
                {
                    FeedbackRequired = true,
                    MeetingId = createdMeeting.MeetingId,
                    MeetingNoteContent = "test2",
                    MeetingNoteId = createdMeetingNote2.MeetingNoteId,
                }
            };

            //Act
            List<MeetingNote> resultNotes = await _meetingService.GetMeetingNotesAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEquivalentTo(expectedNotes);

        }

        /// <summary>
        /// проверяем, что GetMeetingNotesAsync возвращает пустую коллекцию, а не ошибку в случае отсутсвия заметок
        /// </summary>
        [Fact]
        public async void GetNotExistMeetingNotes_ShouldReturnEmptyCollection()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act 
            List<MeetingNote> resultNotes = await _meetingService.GetMeetingNotesAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEmpty();
        }

        /// <summary>
        /// проверяем, что GetMeetingFeedbackRequiredNotesAsync возвращают список только с заметками, которые помечены к отправке
        /// </summary>
        [Fact]
        public async void GetMeetingFeedbackRequiredNotesAsync_ShouldReturnCorrectNotes()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingNote createdMeetingNote = new MeetingNote()
            {
                FeedbackRequired = true,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test",
            };
            MeetingNote createdMeetingNote2 = new MeetingNote()
            {
                FeedbackRequired = false,
                MeetingId = createdMeeting.MeetingId,
                MeetingNoteContent = "test2",
            };
            _dataContext.MeetingNotes.AddRange(createdMeetingNote, createdMeetingNote2);
            _dataContext.SaveChanges();

            List<MeetingNote> expectedNotes = new List<MeetingNote>()
            {
                new MeetingNote
                {
                    FeedbackRequired = true,
                    MeetingId = createdMeeting.MeetingId,
                    MeetingNoteContent = "test",
                    MeetingNoteId = createdMeetingNote.MeetingNoteId,
                },
            };

            //Act
            List<MeetingNote> resultNotes = await _meetingService.GetMeetingFeedbackRequiredNotesAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEquivalentTo(expectedNotes);
        }

        /// <summary>
        /// проверяем, что GetMeetingFeedbackRequiredNotes возвращает пустую коллекцию, а не ошибку в случае отсутсвия заметок
        /// </summary>
        [Fact]
        public async void GetMeetingFeedbackRequiredNotes_ShouldReturnEmptyCollection()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act 
            List<MeetingNote> resultNotes = await _meetingService.GetMeetingFeedbackRequiredNotesAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEmpty();
        }
        #endregion MeetinNote

        #region MeetinGoal
        /// <summary>
        /// проверяем что AddGoalAsync добавляет цель
        /// </summary>
        [Fact]
        public async void AddGoalAsync_ShouldAddNote()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };

            //Act
            await _meetingService.AddGoalAsync(createdMeetingGoal);

            MeetingGoal expectedNote = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
                MeetingGoalId = createdMeetingGoal.MeetingGoalId,
            };

            //Assert
            createdMeetingGoal.Should().BeEquivalentTo(expectedNote);
        }

        /// <summary>
        /// проверяем, что при попытке добавить цель на несуществующую встречу получим ошибку
        /// </summary>
        [Fact]
        public async void AddGoalToNotExistsMeeting_ShouldThrowException()
        {
            //Arrange
            Guid notExistsMeetingId = new Guid();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = notExistsMeetingId,
                MeetingGoalDescription = "test",
            };

            //Act
            var result = async () => await _meetingService.AddGoalAsync(createdMeetingGoal);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Meeting not found");
        }

        /// <summary>
        /// проверяем что UpdateGoalAsync обновляет цель
        /// </summary>
        [Fact]
        public async void UpdateGoalAsync_ShouldUpdateGoal()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };
            _dataContext.MeetingGoals.Add(createdMeetingGoal);
            _dataContext.SaveChanges();

            //Act
            MeetingGoal updatedGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test2",
                MeetingGoalId = createdMeetingGoal.MeetingGoalId,
            };
            await _meetingService.UpdateGoalAsync(updatedGoal);

            MeetingGoal expectedGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test2",
                MeetingGoalId = createdMeetingGoal.MeetingGoalId,
            };

            //Assert
            updatedGoal.Should().BeEquivalentTo(expectedGoal);
        }

        /// <summary>
        /// проверяем что при попытке обновить несуществующую цель получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistGoal_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;
            Guid notExistGoalId = new Guid();

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            MeetingGoal updatedGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test2",
                MeetingGoalId = notExistGoalId,
            };

            //Act
            var result = async () => await _meetingService.UpdateGoalAsync(updatedGoal);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Goal not found");
        }

        /// <summary>
        /// проверяем, что DeleteGoalAsync удаляет цель
        /// </summary>
        [Fact]
        public async void DeleteGoalAsync_ShouldDeleteGoal()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };
            _dataContext.MeetingGoals.Add(createdMeetingGoal);
            _dataContext.SaveChanges();

            //Act
            await _meetingService.DeleteGoalAsync(createdMeetingGoal.MeetingGoalId);
            MeetingGoal deletedGoal = _dataContext.MeetingGoals.Find(createdMeetingGoal.MeetingGoalId);

            //Assert
            deletedGoal.Should().BeNull();
        }

        /// <summary>
        /// проверяем что при попытке удалить несуществующую цель получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistGoal_ShouldThrowException()
        {
            //Arrange
            Guid notExistGoal = new Guid();

            //Act
            var result = async () => await _meetingService.DeleteGoalAsync(notExistGoal);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Goal not found");
        }

        /// <summary>
        /// проверяем что GetMeetingGoalsAsync возвращает корректный список целей
        /// </summary>
        [Fact]
        public async void GetMeetingGoalsAsync_ShouldReturnCorrectGoals()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };
            MeetingGoal createdMeetingGoal2 = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test2",
            };
            _dataContext.MeetingGoals.AddRange(createdMeetingGoal, createdMeetingGoal2);
            _dataContext.SaveChanges();

            List<MeetingGoal> expectedGoals = new List<MeetingGoal>()
            {
                new MeetingGoal
                {
                    MeetingId = createdMeeting.MeetingId,
                    MeetingGoalDescription = "test",
                    MeetingGoalId = createdMeetingGoal.MeetingGoalId,
                },
                new MeetingGoal()
                {
                    MeetingId = createdMeeting.MeetingId,
                    MeetingGoalDescription = "test2",
                    MeetingGoalId = createdMeetingGoal2.MeetingGoalId,
                }
            };

            //Act
            List<MeetingGoal> resultNotes = await _meetingService.GetMeetingGoalsAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEquivalentTo(expectedGoals);
        }

        /// <summary>
        /// проверяем, что GetMeetingGoalsAsync возвращает пустую коллекцию, а не ошибку в случае отсутсвия целей
        /// </summary>
        [Fact]
        public async void GetNotExistMeetingGoals_ShouldReturnEmptyCollection()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act 
            List<MeetingGoal> resultNotes = await _meetingService.GetMeetingGoalsAsync(createdMeeting.MeetingId);

            //Assert
            resultNotes.Should().BeEmpty();
        }

        /// <summary>
        /// проверяем что CompleteGoalAsync проставляет флаг завершённости
        /// </summary>
        [Fact]
        public async void CompleteGoalAsync_ShouldCompleteGoal()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };
            _dataContext.MeetingGoals.Add(createdMeetingGoal);
            _dataContext.SaveChanges();

            //Act
            await _meetingService.CompleteGoalAsync(createdMeetingGoal.MeetingGoalId);

            MeetingGoal expectedGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
                MeetingGoalId = createdMeetingGoal.MeetingGoalId,
                IsCompleted = true,
            };

            //Assert
            createdMeetingGoal.Should().BeEquivalentTo(expectedGoal);
        }

        /// <summary>
        /// проверяем что при попытке завершить несуществующю цель получим ошибку
        /// </summary>
        [Fact]
        public async void CompleteGoalOnNotExistsMeeting_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;
            Guid notExistGoalId = Guid.NewGuid();

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            var result = async () => await _meetingService.CompleteGoalAsync(notExistGoalId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Goal not found");
        }
        #endregion MeetinGoal

        #region MeetingProcessing
        /// <summary>
        /// проверяем, что GetLastOneToOneByPersonIdAsync возвращает последнюю встречу
        /// </summary>
        [Fact]
        public async void GetLastOneToOneByPersonIdAsync_ShouldReturnLastMeeting()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(+1);

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
                MeetingPlanDate = meetingDate,
                MeetingDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                MeetingDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting, createdMeeting2);
            _dataContext.SaveChanges();

            Meeting expectedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                MeetingDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
                MeetingId = createdMeeting2.MeetingId,
            };

            //Act
            Meeting resultMeeting = await _meetingService.GetLastOneToOneByPersonIdAsync(createdPerson.PersonId);

            //Assert
            resultMeeting.Should().BeEquivalentTo(expectedMeeting);
        }

        /// <summary>
        /// проверяем, что GetLastOneToOneByPersonIdAsync возвращает пустое значение, если не было проведённых встреч
        /// </summary>
        [Fact]
        public async void GetLastOneToOneByPersonIdAsync_ShouldReturnEmptyIfMeetingsNotExists()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(+1);

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
            Meeting resultMeeting = await _meetingService.GetLastOneToOneByPersonIdAsync(createdPerson.PersonId);

            //Assert
            resultMeeting.Should().BeNull();
        }

        /// <summary>
        /// Проверяем что MarkAsSendedFollowUpAndFillActualDateAsync выставляет флаг, что фолоуапп был отправлен и проставляет входящую дату
        /// </summary>
        [Fact]
        public async void MarkAsSendedFollowUpAndFillActualDateAsync_ShouldMarkAsSendedAndFillActualDate()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime actualMeetingDate = DateTime.Now.AddHours(5);

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
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();


            Meeting expectedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                MeetingDate = actualMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = true,
                MeetingId = createdMeeting.MeetingId,
            };

            //Act
            Meeting result = await _meetingService.MarkAsSendedFollowUpAndFillActualDateAsync(createdMeeting.MeetingId, actualMeetingDate);

            //Assert
            result.Should().BeEquivalentTo(expectedMeeting);
        }

        /// <summary>
        /// Проверяем что MarkAsSendedFollowUpAndFillActualDateAsync при попытке обновить несуществующую встречу выкинет ошибку
        /// </summary>
        [Fact]
        public async void MarkAsSendedFollowUpAndFillActualDate_ShouldThrowExceptionIfMeetingNotExists()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime actualMeetingDate = DateTime.Now.AddHours(2);

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

            Meeting updatedMeeting = new Meeting()
            {
                MeetingPlanDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = true,
                MeetingId = new Guid(),
            };

            //Act
            var result = async () => await _meetingService.MarkAsSendedFollowUpAndFillActualDateAsync(updatedMeeting.MeetingId, actualMeetingDate);

            //Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Meeting not found");
        }

        /// <summary>
        /// проверяем, что GetPreviousMeetingIdAsync возвращает идентификатор предыдущей встречи
        /// </summary>
        [Fact]
        public async void GetPreviousMeetingIdAsync_ShouldReturnPreviousMeetingID()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;
            DateTime meetingDate2 = DateTime.Now.AddDays(+1);

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
                MeetingPlanDate = meetingDate,
                MeetingDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = meetingDate2,
                MeetingDate = meetingDate2,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };
            _dataContext.Meetings.AddRange(createdMeeting, createdMeeting2);
            _dataContext.SaveChanges();

            Guid? expectedMeetingId = createdMeeting2.MeetingId;

            //Act
            Guid? resultMeetingId = await _meetingService.GetPreviousMeetingIdAsync(createdMeeting.MeetingId, createdPerson.PersonId);

            //Assert
            resultMeetingId.Should().Be(expectedMeetingId);
        }

        /// <summary>
        /// проверяем, что GetPreviousMeetingIdAsync возвращает null если не было предыдущей встречи
        /// </summary>
        [Fact]
        public async void GetPreviousMeetingIdAsync_ShouldReturnNullIfPreviousMeetingsNotExists()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime meetingDate = DateTime.Now;

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
                MeetingPlanDate = meetingDate,
                MeetingDate = meetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            Guid? resultMeetingId = await _meetingService.GetPreviousMeetingIdAsync(createdMeeting.MeetingId, createdPerson.PersonId);

            //Assert
            resultMeetingId.Should().BeNull();
        }

        /// <summary>
        /// проверяем, что GetMeetingGoalsByPersonAsync возвращает цели по конкретному оструднику
        /// </summary>
        [Fact]
        public async void GetMeetingGoalsByPersonAsync_ShouldReturnPersonsGoals()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test",
            };
            MeetingGoal createdMeetingGoal2 = new MeetingGoal()
            {
                MeetingId = createdMeeting.MeetingId,
                MeetingGoalDescription = "test2",
            };
            _dataContext.MeetingGoals.AddRange(createdMeetingGoal, createdMeetingGoal2);
            _dataContext.SaveChanges();

            Person createdPerson2 = new Person()
            {
                Email = "1111@test2.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(createdPerson2);
            _dataContext.SaveChanges();
            Meeting createdMeeting2 = new Meeting()
            {
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson2.PersonId,
                Person = createdPerson2,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting2);
            _dataContext.SaveChanges();

            MeetingGoal createdMeetingGoal3 = new MeetingGoal()
            {
                MeetingId = createdMeeting2.MeetingId,
                MeetingGoalDescription = "test",
            };
            MeetingGoal createdMeetingGoal4 = new MeetingGoal()
            {
                MeetingId = createdMeeting2.MeetingId,
                MeetingGoalDescription = "test2",
            };
            _dataContext.MeetingGoals.AddRange(createdMeetingGoal3, createdMeetingGoal4);
            _dataContext.SaveChanges();

            List<MeetingGoal> expectedGoals = new List<MeetingGoal>()
            {
                new MeetingGoal
                {
                    MeetingId = createdMeeting.MeetingId,
                    MeetingGoalDescription = "test",
                    MeetingGoalId = createdMeetingGoal.MeetingGoalId,
                },
                new MeetingGoal()
                {
                    MeetingId = createdMeeting.MeetingId,
                    MeetingGoalDescription = "test2",
                    MeetingGoalId = createdMeetingGoal2.MeetingGoalId,
                }
            };

            //Act
            List<MeetingGoal> resultNotes = await _meetingService.GetMeetingGoalsByPersonAsync(createdPerson.PersonId);

            //Assert
            resultNotes.Should().BeEquivalentTo(expectedGoals);
        }

        /// <summary>
        /// проверяем, что GetMeetingGoalsByPersonAsync возвращает пустую коллекцию, если не было заведено целей во время встречи
        /// </summary>
        [Fact]
        public async void GetMeetingGoalsByPersonAsync_ShouldReturnEmptyCollectionIfGoalsNotExists()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            List<MeetingGoal> resultNotes = await _meetingService.GetMeetingGoalsByPersonAsync(createdPerson.PersonId);

            //Assert
            resultNotes.Should().BeEmpty();
        }

        /// <summary>
        /// проверяем, что  GetFactMeetingDateByIdAsync возвращает верную дату проведения встречи по её идентификатору
        /// </summary>
        [Fact]
        public async void GetFactMeetingDateByIdAsync_ShouldReturnCorrectDateTime()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            //очень важно пожать только дату, т.к. при сохранении в БД могут теряться тысячные секунды и тест завалится 100%, хотя у нас нет таких критериев к точности
            DateTime plannedMeetingDate = DateTime.Now.Date;
            DateTime actualMeetingDate = DateTime.Now.Date;

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
                MeetingPlanDate = plannedMeetingDate,
                MeetingDate = actualMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            DateTime? result = await _meetingService.GetFactMeetingDateByIdAsync(createdMeeting.MeetingId);

            //Assert
            result.Should().Be(actualMeetingDate);
        }

        /// <summary>
        /// проверяем, что  GetFactMeetingDateByIdAsync возвращает пустую дату, если встреча не была проведена
        /// </summary>
        [Fact]
        public async void GetFactMeetingDateByIdAsync_ShouldReturnEmptyDateTimeIfItNotProcessed()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            DateTime plannedMeetingDate = DateTime.Now;

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
                MeetingPlanDate = plannedMeetingDate,
                PersonId = createdPerson.PersonId,
                Person = createdPerson,
                FollowUpIsSended = false,
            };

            _dataContext.Meetings.Add(createdMeeting);
            _dataContext.SaveChanges();

            //Act
            DateTime? result = await _meetingService.GetFactMeetingDateByIdAsync(createdMeeting.MeetingId);

            //Assert
            result.Should().BeNull();
        }
        #endregion MeetingProcessing
    }
}

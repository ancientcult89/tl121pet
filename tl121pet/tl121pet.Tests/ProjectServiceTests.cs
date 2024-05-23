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
    public class ProjectServiceTests : IDisposable
    {
        private DataContext _dataContext;
        private IProjectService _projectService;
        public ProjectServiceTests()
        {
            var connectionString = "Server=localhost;Database=TLProjectTest;Port=49154;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            _projectService = new ProjectService(_dataContext);
        }

        public void Dispose()
        {
            _dataContext.Database.EnsureDeleted();
        }

        /// <summary>
        /// проверяем, что GetAllTeamsAsync возвращает все проекты
        /// </summary>
        [Fact]
        public async void GetAllTeamsAsync_ShouldReturnAllProjects()
        {
            //Arrange
            ProjectTeam testProject1 = new ProjectTeam { ProjectTeamName = "SABPEK"};
            ProjectTeam testProject2 = new ProjectTeam { ProjectTeamName = "SIDE" };
            await _projectService.CreateProjectTeamAsync(testProject1);
            await _projectService.CreateProjectTeamAsync(testProject2);

            List<ProjectTeam> expectedTeams = new List<ProjectTeam>() {
                new ProjectTeam { ProjectTeamName = "SABPEK", ProjectTeamId = testProject1.ProjectTeamId },
                new ProjectTeam { ProjectTeamName = "SIDE", ProjectTeamId = testProject2.ProjectTeamId },
            };

            //Act
            var resultProjects = await _projectService.GetAllTeamsAsync();

            //Assert
            resultProjects.Should().BeEquivalentTo(expectedTeams);
        }

        /// <summary>
        /// проверяем, что GetAllTeamsAsync возвращает пустую коллекцию, если нет ни одного проекта
        /// </summary>
        [Fact]
        public async void GetAllTeamsAsync_SHouldReturnEmptyCollectionIfProjectsNotExists()
        { 
            //Arrange
            List<ProjectTeam> expectedTeams = new List<ProjectTeam>();

            //Act
            var resultProjects = await _projectService.GetAllTeamsAsync();

            //Assert
            resultProjects.Should().BeEquivalentTo(expectedTeams);
        }

        /// <summary>
        /// проверяем, что GetProjectTeamByIdAsync возвращает корректный проект
        /// </summary>
        [Fact]
        public async void GetProjectTeamByIdAsync_ShouldReturnCorrectProject()
        {
            //Arrange
            ProjectTeam testProject1 = new ProjectTeam { ProjectTeamName = "SABPEK" };
            await _projectService.CreateProjectTeamAsync(testProject1);
            ProjectTeam expectedProject = new ProjectTeam { ProjectTeamName = "SABPEK", ProjectTeamId = testProject1.ProjectTeamId };

            //Act
            var resultProjects = await _projectService.GetProjectTeamByIdAsync(testProject1.ProjectTeamId);

            //Assert
            resultProjects.Should().BeEquivalentTo(expectedProject);
        }

        /// <summary>
        /// проверяем, что при попытке получить через GetProjectTeamByIdAsync несуществующую запись получим ошибку
        /// </summary>
        [Fact]
        public async void GetNotExistProject_ShouldThrowException()
        {
            //Arrange
            var notExistProjectId = 1;

            //Act
            var result = async () => await _projectService.GetProjectTeamByIdAsync(notExistProjectId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Project not found");
        }

        /// <summary>
        /// Убеждаемся, что DeleteProjectTeamAsync удаляет нужный объект
        /// </summary>
        [Fact]
        public async void DeleteProjectTeamAsync_ShouldDeletePerson()
        {
            //Arrange
            ProjectTeam testProject1 = new ProjectTeam { ProjectTeamName = "SABPEK" };
            await _projectService.CreateProjectTeamAsync(testProject1);

            //Act
            long deletedId = testProject1.ProjectTeamId;
            await _projectService.DeleteProjectTeamAsync(testProject1.ProjectTeamId);
            ProjectTeam deletedProject = _dataContext.ProjectTeams.Find(deletedId);

            //Assert
            deletedProject.Should().BeNull();
        }

        /// <summary>
        /// проверяем, что при попытке удалить несуществующую запись получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistProject_ShouldThrowException()
        {
            //Arrange
            long notExistProjectId = 1;

            //Act
            var result = async() => await _projectService.DeleteProjectTeamAsync(notExistProjectId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Project not found");
        }

        /// <summary>
        /// проверяем, что CreateProjectTeamAsync создаёт корректный проект
        /// </summary>
        [Fact]
        public async void CreateProjectTeamAsync_ShouldCreateProject()
        {
            //Arrange
            ProjectTeam project = new ProjectTeam { ProjectTeamName = "Test" };
            ProjectTeam expectedProject = new ProjectTeam { ProjectTeamName = "Test" };


            //Act
            await _projectService.CreateProjectTeamAsync(project);
            expectedProject.ProjectTeamId = project.ProjectTeamId;

            //Assert
            project.Should().BeEquivalentTo(expectedProject);
        }

        /// <summary>
        /// проверяем, что невозможно завести второй проект с одним и тем же названием
        /// </summary>
        [Fact]
        public async void CreateDuplicatedProject_ShouldThrowException()
        {
            //Arrange
            ProjectTeam project = new ProjectTeam { ProjectTeamName = "Test" };
            ProjectTeam duplicatedProject = new ProjectTeam { ProjectTeamName = "Test" };
            await _projectService.CreateProjectTeamAsync(project);


            //Act
            var result = async () => await _projectService.CreateProjectTeamAsync(duplicatedProject);

            //Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("A Project with same name is already exists");
        }

        /// <summary>
        /// убеждаемся что UpdateProjectTeamAsync корректно обновляет запись
        /// </summary>
        [Fact]
        public async void UpdateProjectTeamAsync_ShouldUpdateProject()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            await _projectService.CreateProjectTeamAsync(sourseTeam);
            ProjectTeam expectedTeam = new ProjectTeam { ProjectTeamName = "Test1", ProjectTeamId = sourseTeam.ProjectTeamId };
            ProjectTeam updatedTeam = new ProjectTeam { ProjectTeamName = "Test1", ProjectTeamId = sourseTeam.ProjectTeamId };

            //Act
            await _projectService.UpdateProjectTeamAsync(updatedTeam);

            //Assert
            updatedTeam.Should().BeEquivalentTo(expectedTeam);
        }

        /// <summary>
        /// проверяем, что при попытке обновить несуществующую запись получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistProject_ShouldThrowException()
        {
            //Arrange
            ProjectTeam notExistTeam = new ProjectTeam { ProjectTeamName = "Test", ProjectTeamId = 3 };

            //Act
            var result = async() => await _projectService.UpdateProjectTeamAsync(notExistTeam);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Project not found");
        }

        /// <summary>
        /// проверяем, что при попытке сменить название на такое же, которое уже существует получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateProjectWithDuplicatedName_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            await _projectService.CreateProjectTeamAsync(sourseTeam);
            ProjectTeam updatedTeam = new ProjectTeam { ProjectTeamName = "Test", ProjectTeamId = sourseTeam.ProjectTeamId };

            //Act
            var result = async() => await _projectService.UpdateProjectTeamAsync(updatedTeam);

            //Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("A Project with same name is already exists");
        }

        /// <summary>
        /// проверяем что AddPersonMembershipAsync добавляет запись участия в проекте
        /// </summary>
        [Fact]
        public async void AddPersonMembershipAsync_ShouldAddPersonMemberShip()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            await _projectService.CreateProjectTeamAsync(sourseTeam);
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
            ProjectMember addedProjectMember = await _projectService.AddPersonMembershipAsync(createdPerson.PersonId, sourseTeam.ProjectTeamId);
            ProjectMember expectedProjectMember = new ProjectMember { 
                PersonId = createdPerson.PersonId,
                ProjectTeamId = sourseTeam.ProjectTeamId,
                ProjectMemberId = addedProjectMember.ProjectMemberId,
                Person = createdPerson,
                ProjectTeam = sourseTeam,
            };

            //Assert
            addedProjectMember.Should().BeEquivalentTo(expectedProjectMember);
        }

        /// <summary>
        /// проверяем что при попытке повторно добавить сотрудника в проект получим ошибку
        /// </summary>
        [Fact]
        public async void AddDuplicatedPersonMembership_ShouldThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            await _projectService.CreateProjectTeamAsync(sourseTeam);
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
            ProjectMember addedProjectMember = await _projectService.AddPersonMembershipAsync(createdPerson.PersonId, sourseTeam.ProjectTeamId);

            //Act
            var result = async () => await _projectService.AddPersonMembershipAsync(createdPerson.PersonId, sourseTeam.ProjectTeamId);


            //Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("The Project is already used");
        }

        /// <summary>
        /// проверяем, что GetPersonMembershipAsync возвращает правильный список проектов по сотруднику
        /// </summary>
        [Fact]
        public async void GetPersonMembershipAsync_ShouldReturnCorrectPersonProjects()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.AddRange(sourseTeam, sourseTeam2);
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

            ProjectMember pm1 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam.ProjectTeamId };
            ProjectMember pm2 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam2.ProjectTeamId };
            _dataContext.ProjectMembers.Add(pm1);
            _dataContext.ProjectMembers.Add(pm2);
            _dataContext.SaveChanges();

            List<ProjectTeam> expectedProjects = new List<ProjectTeam>() { 
                new ProjectTeam { ProjectTeamId = sourseTeam.ProjectTeamId,ProjectTeamName = sourseTeam.ProjectTeamName },
                new ProjectTeam { ProjectTeamId = sourseTeam2.ProjectTeamId,ProjectTeamName = sourseTeam2.ProjectTeamName },
            };

            //Act
            var resultPersonProjects = await _projectService.GetPersonMembershipAsync(createdPerson.PersonId);

            //Assert
            resultPersonProjects.Should().BeEquivalentTo(expectedProjects);
        }

        /// <summary>
        /// проверяем, что GetPersonMembershipAsync возвращает пустую коллекцию, если нет ни одного проекта
        /// </summary>
        [Fact]
        public async void GetPersonMembershipAsync_ShouldReturnEmptyCollectionIfProjectsNotExists()
        {
            //Arrange
            List<ProjectTeam> expectedProjects = new();
            long personId = 1;

            //Act
            var resultPersonProjects = await _projectService.GetPersonMembershipAsync(personId);

            //Assert
            resultPersonProjects.Should().BeEquivalentTo(expectedProjects);
        }

        /// <summary>
        /// проверяем, что GetPersonsProjectsAsStringAsync возвращает корректную строку со списком проектов
        /// </summary>
        [Fact]
        public async void GetPersonsProjectsAsStringAsync_ShouldReturnCorrectProjectsString()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.AddRange(sourseTeam, sourseTeam2);
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

            ProjectMember pm1 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam.ProjectTeamId };
            ProjectMember pm2 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam2.ProjectTeamId };
            _dataContext.ProjectMembers.Add(pm1);
            _dataContext.ProjectMembers.Add(pm2);
            _dataContext.SaveChanges();
            string expectedProjectsString = "Test; Test2; ";

            //Act
            var resultPersonProjects = await _projectService.GetPersonsProjectsAsStringAsync(createdPerson.PersonId);

            //Assert
            resultPersonProjects.Should().BeEquivalentTo(expectedProjectsString);
        }

        /// <summary>
        /// проверяем, что GetPersonsProjectsAsStringAsync возвращает пустую строку, если нет ни одного проекта
        /// </summary>
        [Fact]
        public async void GetPersonsProjectsAsStringAsync_ShouldReturnEmptyStringProjectsNotExists()
        {
            //Arrange
            string expectedStringProjects = "";
            long personId = 1;

            //Act
            var resultPersonProjects = await _projectService.GetPersonsProjectsAsStringAsync(personId);

            //Assert
            resultPersonProjects.Should().BeEquivalentTo(expectedStringProjects);
        }

        /// <summary>
        /// проверяем, что DeletePersonMembershipAsync должен удалять участие сотрудника в проекте
        /// </summary>
        [Fact]
        public async void DeletePersonMembershipAsync_ShouldDeletePersonsProject()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.AddRange(sourseTeam, sourseTeam2);
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

            ProjectMember pm1 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam.ProjectTeamId };
            ProjectMember pm2 = new ProjectMember { PersonId = createdPerson.PersonId, ProjectTeamId = sourseTeam2.ProjectTeamId };
            _dataContext.ProjectMembers.Add(pm1);
            _dataContext.ProjectMembers.Add(pm2);
            _dataContext.SaveChanges();

            List<ProjectTeam> expectedProjects = new List<ProjectTeam>() {
                new ProjectTeam { ProjectTeamId = sourseTeam.ProjectTeamId,ProjectTeamName = sourseTeam.ProjectTeamName },
            };

            //Act
            await _projectService.DeletePersonMembershipAsync(createdPerson.PersonId, sourseTeam2.ProjectTeamId);
            var resultPersonProjects = await _projectService.GetPersonMembershipAsync(createdPerson.PersonId);

            //Assert
            resultPersonProjects.Should().BeEquivalentTo(expectedProjects);
        }

        /// <summary>
        /// проверяем, что при попытке удалить несуществующий проект сотрудника получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsPersonMembershipAsync_ShouldThrowException()
        {
            //Arrange
            long notExistPersonId = 1;
            long notExistProjectId = 1;

            //Act
            var result = async () => await _projectService.DeletePersonMembershipAsync(notExistPersonId, notExistProjectId);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("The Persons Project not exist");
        }

        /// <summary>
        /// Проверяем что AddUserMembershipAsync добавляет проект пользователю
        /// </summary>
        [Fact]
        public async void AddUserMembershipAsync_ShouldAddProjectToUser()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            _dataContext.ProjectTeams.Add(sourseTeam);
            _dataContext.SaveChanges();

            User user = new User {
                UserName = "Test",
                Email = "test@test.test",                
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            //Act
            var resultUserProject = await _projectService.AddUserMembershipAsync(user.Id, sourseTeam.ProjectTeamId);
            UserProject expectedUserProject = new UserProject() { 
                ProjectTeamId = sourseTeam.ProjectTeamId,
                ProjectTeam = sourseTeam,
                User = user,
                UserId = user.Id,
                UserProjectId = resultUserProject.UserId
            };

            //Assert
            resultUserProject.Should().BeEquivalentTo(expectedUserProject);
        }

        /// <summary>
        /// Проверяем что при попытке добавить уже существующий у пользователя проект получим ошибку
        /// </summary>
        [Fact]
        public async void AddExistsUserMembership_ShouldAddThrowException()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            _dataContext.ProjectTeams.Add(sourseTeam);
            _dataContext.SaveChanges();

            User user = new User
            {
                UserName = "Test",
                Email = "test@test.test",
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
            var firstUserProject = await _projectService.AddUserMembershipAsync(user.Id, sourseTeam.ProjectTeamId);

            //Act
            var resultUserProject= async () => await _projectService.AddUserMembershipAsync(user.Id, sourseTeam.ProjectTeamId);

            //Assert
            await resultUserProject.Should().ThrowAsync<LogicException>().WithMessage("The Project is already used");
        }

        /// <summary>
        /// проверяем, что GetUserMembershipAsync возвращает корректный список проектов пользователя
        /// </summary>
        [Fact]
        public async void GetUserMembershipAsync_ShouldReturnCorrectProjects()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            _dataContext.ProjectTeams.Add(sourseTeam);
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.Add(sourseTeam2);
            _dataContext.SaveChanges();

            User user = new User
            {
                UserName = "Test",
                Email = "test@test.test",
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            UserProject firstUserProject = new UserProject() {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam,
                ProjectTeamId = sourseTeam.ProjectTeamId
            };
            _dataContext.UserProjects.Add(firstUserProject);

            UserProject secondUserProject = new UserProject()
            {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam2,
                ProjectTeamId = sourseTeam2.ProjectTeamId
            };
            _dataContext.UserProjects.Add(secondUserProject);
            _dataContext.SaveChanges();

            List<ProjectTeam> expectedProjects = new List<ProjectTeam>()
            {
                new ProjectTeam {
                    ProjectTeamId= sourseTeam.ProjectTeamId,
                    ProjectTeamName = "Test"
                },
                new ProjectTeam {
                    ProjectTeamId= sourseTeam2.ProjectTeamId,
                    ProjectTeamName = "Test2"
                },
            };

            //Act
            var resultUserProjects = await _projectService.GetUserMembershipAsync(user.Id);

            //Assert
            resultUserProjects.Should().BeEquivalentTo(expectedProjects);
        }

        /// <summary>
        /// проверяем, что при отсутсвии проектов у пользователя получим пустую коллекцию
        /// </summary>
        [Fact]
        public async void GetUserMembershipAsync_ShouldReturnEmptyCollectionIfProjectsNotExists()
        {
            //Arrange
            long userId = 1;

            //Act
            var result = await _projectService.GetUserMembershipAsync(userId);

            //Assert
            result.Should().BeEmpty();
        }

        /// <summary>
        /// проверяем, что DeleteUserMembershipAsync удаляет проект с пользователя
        /// </summary>
        [Fact]
        public async void DeleteUserMembershipAsync_ShouldDeleteUsersProject()
        {
            //Arrange
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            _dataContext.ProjectTeams.Add(sourseTeam);
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.Add(sourseTeam2);
            _dataContext.SaveChanges();

            User user = new User
            {
                UserName = "Test",
                Email = "test@test.test",
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            UserProject firstUserProject = new UserProject()
            {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam,
                ProjectTeamId = sourseTeam.ProjectTeamId
            };
            _dataContext.UserProjects.Add(firstUserProject);

            UserProject secondUserProject = new UserProject()
            {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam2,
                ProjectTeamId = sourseTeam2.ProjectTeamId
            };
            _dataContext.UserProjects.Add(secondUserProject);
            _dataContext.SaveChanges();

            List<ProjectTeam> expectedProjects = new List<ProjectTeam>()
            {
                new ProjectTeam {
                    ProjectTeamId= sourseTeam.ProjectTeamId,
                    ProjectTeamName = "Test"
                },
            };

            //Act
            await _projectService.DeleteUserMembershipAsync(user.Id, sourseTeam2.ProjectTeamId);
            var result = await _projectService.GetUserMembershipAsync(user.Id);

            //Assert
            result.Should().BeEquivalentTo(expectedProjects);
        }

        /// <summary>
        /// Проверяем что при попытке удалить проект, которого нет у пользователя получим ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistUserProject_ShouldThrowException()
        {
            //Arrange
            long notExistUser = 1;
            long notExistUserProject = 1;

            //Act
            var result = async () => await _projectService.DeleteUserMembershipAsync(notExistUser, notExistUserProject);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("The User Project not exist");
        }

        /// <summary>
        /// Проверяем, что GetUserProjectsNameAsync возвращает строку с корректным списком проектов
        /// </summary>
        [Fact]
        public async void GetUserProjectsNameAsync_ShouldReturnCorrectProjectsString()
        {
            //Arrange
            string expectedUsersProjectString = "Test; Test2; ";
            ProjectTeam sourseTeam = new ProjectTeam { ProjectTeamName = "Test" };
            _dataContext.ProjectTeams.Add(sourseTeam);
            ProjectTeam sourseTeam2 = new ProjectTeam { ProjectTeamName = "Test2" };
            _dataContext.ProjectTeams.Add(sourseTeam2);
            _dataContext.SaveChanges();

            User user = new User
            {
                UserName = "Test",
                Email = "test@test.test",
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            UserProject firstUserProject = new UserProject()
            {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam,
                ProjectTeamId = sourseTeam.ProjectTeamId
            };
            _dataContext.UserProjects.Add(firstUserProject);

            UserProject secondUserProject = new UserProject()
            {
                User = user,
                UserId = user.Id,
                ProjectTeam = sourseTeam2,
                ProjectTeamId = sourseTeam2.ProjectTeamId
            };
            _dataContext.UserProjects.Add(secondUserProject);
            _dataContext.SaveChanges();

            //Act
            string result = await _projectService.GetUserProjectsNameAsync(user.Id);

            //Assert
            result.Should().BeEquivalentTo(expectedUsersProjectString);
        }

        /// <summary>
        /// Проверяем что при попытке получить список проектов по пользователю у которого их нет мы получим пустую строку
        /// </summary>
        [Fact]
        public async void GetNotExistsUsersProject_ShouldReturnEmptyString()
        {
            //Arrange
            long userId = 1;
            string expectedUserProjectString = "";

            //Act
            var result = await _projectService.GetUserProjectsNameAsync(userId);

            //Assert
            result.Should().BeEquivalentTo(expectedUserProjectString);
        }
    }
}

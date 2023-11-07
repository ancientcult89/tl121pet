using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using tl121pet.DAL.Data;
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
            var connectionString = "Server=host.docker.internal;Database=TLProjectTest;Port=49153;User Id=postgres;Password=postgrespw";
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
            await result.Should().ThrowAsync<Exception>().WithMessage("Project not found");
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
            await result.Should().ThrowAsync<Exception>().WithMessage("Project not found");
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
            await result.Should().ThrowAsync<Exception>().WithMessage("A Project with same name is already exists");
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
            await result.Should().ThrowAsync<Exception>().WithMessage("Project not found");
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
            await result.Should().ThrowAsync<Exception>().WithMessage("A Project with same name is already exists");
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
            await result.Should().ThrowAsync<Exception>().WithMessage("The Project is already used");
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
        /// проверяем, что GetPersonMembershipAsync возвращает пустую строку, если нет ни одного проекта
        /// </summary>
        [Fact]
        public async void GetPersonsProjectsAsStringAsync_ShouldReturnEmptyStringProjectsNotExists()
        {
            //Arrange
            string expectedStringProjects = "";
            long personId = 1;

            //Act
            var resultPersonProjects = await _projectService.GetPersonMembershipAsync(personId);

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
            await result.Should().ThrowAsync<Exception>().WithMessage("The Persons Project not exist");
        }
    }
}

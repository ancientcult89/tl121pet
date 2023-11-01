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
    }
}

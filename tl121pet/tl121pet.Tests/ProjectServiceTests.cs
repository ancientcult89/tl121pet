﻿using FluentAssertions;
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
            _dataContext.ProjectTeams.AddRange(testProject1, testProject2);
            _dataContext.SaveChanges();

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
        public async void GetAllTeamsAsync_SHouldReturnEmptyCOllectionIfProjectsNotExists()
        { 
            //Arrange
            List<ProjectTeam> expectedTeams = new List<ProjectTeam>();

            //Act
            var resultProjects = await _projectService.GetAllTeamsAsync();

            //Assert
            resultProjects.Should().BeEquivalentTo(expectedTeams);
        }
    }
}
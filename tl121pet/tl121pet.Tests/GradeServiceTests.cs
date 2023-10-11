﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using tl121pet.Tests.TestData;
using Xunit;

namespace tl121pet.Tests
{
    public class GradeServiceTests
    {
        private readonly DataContext _dataContext;
        private readonly IGradeService _gradeService;
        public GradeServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "GradeServiceConnection")
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _gradeService = new GradeService(_dataContext);
        }

        /// <summary>
        /// Проверка, что GetAllGradesAsync возвращает все доступные грейды
        /// </summary>
        [Fact]
        public async void GetAllGradesAsync_ShouldReturnAllGrades()
        {
            //Arrange
            _dataContext.Grades.AddRange((IEnumerable<Grade>)GradeTestData.GetTestGrades());
            _dataContext.SaveChanges();

            //Act
            IEnumerable<Grade> grades = await _gradeService.GetAllGradesAsync();
            IEnumerable<Grade> resultGrades = (IEnumerable<Grade>)GradeTestData.GetTestGrades();

            //Assert
            resultGrades.Should().BeEquivalentTo(grades);
        }

        /// <summary>
        /// Проверка, что GetAllGradesAsync возвращает пустую коллекцию и не вываливается в ошибку, если нет ни одной записи
        /// </summary>
        [Fact]
        public async void GetAllGradesAsync_ShouldReturnEmptyListIfGradesNotExists()
        {
            //Arrange
            List<Grade> expectgrades = new();

            //Act
            IEnumerable<Grade> grades = await _gradeService.GetAllGradesAsync();

            //Assert
            expectgrades.Should().BeEquivalentTo(grades);
        }


        /// <summary>
        /// Проверка, что GetGradeByIdAsync возвращает ожидаемый грейд
        /// </summary>
        [Fact]
        public async void GetGradeByIdAsync_ShouldReturnCorrectGrade()
        {
            //Arrange
            Grade expectGrade = (Grade)GradeTestData.GetSingleGrade();
            _dataContext.Grades.Add(expectGrade);
            _dataContext.SaveChanges();

            //Act
            Grade resultGrade = await _gradeService.GetGradeByIdAsync(expectGrade.GradeId);

            //Assert
            resultGrade.Should().BeEquivalentTo(expectGrade);
        }

        /// <summary>
        /// Проверка, что GetGradeByIdAsync выкинет исключение с нужным текстом, если не найдено грейда с таким ID
        /// </summary>
        [Fact]
        public async void GetGradeByIdAsync_ShouldThrowExceptionIfGradeNotExistsWithCorrectMessage()
        {
            // Act
            var result = async () => await _gradeService.GetGradeByIdAsync(1);

            // Act & Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Grade not found");
        }

        /// <summary>
        /// Тестируем метод CreateGradeAsync, проверяем, что создаётся корректная сущность
        /// </summary>
        [Fact]
        public async void CreateGradeAsync_ShouldCreateGrade()
        {
            //Arrange
            Grade expectGrade = (Grade)GradeTestData.GetSingleGrade();
            await _gradeService.CreateGradeAsync(expectGrade);

            //Act
            Grade resultGrade = await _gradeService.GetGradeByIdAsync(expectGrade.GradeId);

            //Assert
            resultGrade.Should().BeEquivalentTo(expectGrade);
        }

        /// <summary>
        /// Проверка создания грейда, убеждаемся, что при попытке внести грейд с существующим именем получим ошибку
        /// </summary>
        [Fact]
        public async void CreateGradeAsync_CreatingDuplicateShouldThrowExeption()
        {
            //Arrange
            Grade expectGrade = (Grade)GradeTestData.GetSingleGrade();
            await _gradeService.CreateGradeAsync(expectGrade);
            Grade expectGrade2 = (Grade)GradeTestData.GetAnotherSingleGrade();

            // Act
            var result = async () => await _gradeService.CreateGradeAsync(expectGrade2);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("An Grade with this name exists");
        }

        /// <summary>
        /// Проверка создания грейда, убеждаемся, что при попытке внести грейд с существующим именем получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateGradeAsync_ShouldChangeGradeName()
        {
            //Arrange
            Grade testGrade = (Grade)GradeTestData.GetSingleGrade();
            await _gradeService.CreateGradeAsync(testGrade);
            Grade expectGrade = new Grade
            {
                GradeId = testGrade.GradeId,
                GradeName = "Testing Name",
            };

            // Act
            testGrade.GradeName = "Testing Name";
            await _gradeService.UpdateGradeAsync(testGrade);

            // Assert
            testGrade.Should().BeEquivalentTo(expectGrade);
        }
    }
}
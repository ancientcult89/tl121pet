﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using tl121pet.Tests.TestData;
using Xunit;

namespace tl121pet.Tests
{
    public class GradeServiceTests : IDisposable
    {
        private readonly DataContext _dataContext;
        private readonly IGradeService _gradeService;
        public GradeServiceTests()
        {
            var connectionString = "Server=localhost;Database=TLGradeTest;Port=49154;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            _gradeService = new GradeService(_dataContext);
        }
        public void Dispose()
        {
            _dataContext.Database.EnsureDeleted();
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
            Grade expectGrade = GradeTestData.GetSingleGrade();
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
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Grade not found");
        }

        /// <summary>
        /// Тестируем метод CreateGradeAsync, проверяем, что создаётся корректная сущность
        /// </summary>
        [Fact]
        public async void CreateGradeAsync_ShouldCreateGrade()
        {
            //Arrange
            Grade expectGrade = GradeTestData.GetSingleGrade();
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
            Grade expectGrade = GradeTestData.GetSingleGrade();
            await _gradeService.CreateGradeAsync(expectGrade);
            Grade expectGrade2 = GradeTestData.GetAnotherSingleGrade();

            // Act
            var result = async () => await _gradeService.CreateGradeAsync(expectGrade2);

            // Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("A Grade with this name exists");
        }

        /// <summary>
        /// Положительная проверка изменения грейда
        /// </summary>
        [Fact]
        public async void UpdateGradeAsync_ShouldChangeGradeName()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            Grade expectGrade = new Grade
            {
                GradeId = testGrade.GradeId,
                GradeName = "Testing Name",
            };

            // Act
            Grade testValuedGrade = new Grade
            {
                GradeId = testGrade.GradeId,
                GradeName = "Testing Name"
            };
            await _gradeService.UpdateGradeAsync(testValuedGrade);

            // Assert
            testValuedGrade.Should().BeEquivalentTo(expectGrade);
        }

        /// <summary>
        /// Проверка обновления грейда, убеждаемся, что при попытке внести имя, которое уже существует в другой записи получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateGradeAsync_CreatingDuplicateShouldThrowExeption()
        {
            //Arrange
            Grade testedGrade = new Grade
            {
                GradeName = "Junior"
            };
            await _gradeService.CreateGradeAsync(testedGrade);
            Grade testedGrade2 = new Grade
            {
                GradeId = 2,
                GradeName = "Testing Name",
            };
            await _gradeService.CreateGradeAsync(testedGrade2);

            Grade newValuedTestedGrade2 = new Grade
            {
                GradeId = testedGrade2.GradeId,
                GradeName = "Junior",
            };

            // Act
            var result = async () => await _gradeService.UpdateGradeAsync(newValuedTestedGrade2);

            // Assert
            await result.Should().ThrowAsync<LogicException>().WithMessage("A Grade with this name exists");
        }

        /// <summary>
        /// Убеждаемся что при попытке обновить несуществующий грейд мы получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistsGrade_ShouldThrowException()
        { 
            //Arrange
            Grade testGrade = GradeTestData.GetSingleGrade();

            //Act
            var result = async () => await _gradeService.UpdateGradeAsync(testGrade);

            //Assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Grade not found");
        }

        /// <summary>
        /// Проверяем что удаление грейда действительно его удаляет
        /// </summary>
        [Fact]
        public async void DeleteGradeAsync_ShouldDeleteGrade()
        { 
            //arrange
            Grade grade = GradeTestData.GetSingleGrade();
            _dataContext.Grades.Add(grade);
            _dataContext.SaveChanges();

            //act
            await _gradeService.DeleteGradeAsync(grade.GradeId);
            Grade notExistGrade = _dataContext.Grades.Find(grade.GradeId);

            //assert
            notExistGrade.Should().BeNull();
        }

        /// <summary>
        /// Проверяем что при попытке удалить несуществующий грейд получим соответсвующую ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsGrade_ShouldThrowException()
        {
            //arrange
            Grade grade = GradeTestData.GetSingleGrade();

            //act
            var result = async () => await _gradeService.DeleteGradeAsync(grade.GradeId);

            //assert
            await result.Should().ThrowAsync<DataFoundException>().WithMessage("Grade not found");
        }
    }
}

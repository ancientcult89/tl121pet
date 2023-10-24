using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using tl121pet.Tests.TestData;
using Xunit;

namespace tl121pet.Tests
{
    public class PersonServiceTests
    {
        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;
        public PersonServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "RoleServiceConnection")
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _personService = new PersonService(_dataContext);
        }

        /// <summary>
        /// Проверка, что GetAllPeopleAsync возвращает всех сотрудников
        /// </summary>
        [Fact]
        public async void GetAllPeopleAsync_ShouldReturnAllPersons()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            List<Person> people = new List<Person>();
            people.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });
            _dataContext.People.AddRange(people);
            _dataContext.SaveChanges();

            List<Person> peopleExpected = new List<Person>();
            peopleExpected.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });

            //Act
            var resultPeople = await _personService.GetAllPeopleAsync();

            //Assert
            resultPeople.Should().BeEquivalentTo(peopleExpected);
        }

        /// <summary>
        /// Проверка, что GetAllPeopleAsync возвращает пустую коллекцию и не вываливается в ошибку, если нет ни одной записи
        /// </summary>
        [Fact]
        public async void GetAllPeopleAsync_ShouldReturnEmptyListIfPersonsNotExists()
        {
            //Arrange
            List<Person> expectPeople = new();

            //Act
            IEnumerable<Person> people = await _personService.GetAllPeopleAsync();

            //Assert
            expectPeople.Should().BeEquivalentTo(people);
        }
    }
}

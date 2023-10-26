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
    public class PersonServiceTests : IDisposable
    {
        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;
        public PersonServiceTests()
        {
            var connectionString = "Server=host.docker.internal;Database=TLPersonTest;Port=49153;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
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

        /// <summary>
        /// проверяем, что CreatePersonAsync создаёт корректную запись сотрудника
        /// </summary>
        [Fact]
        public async void CreatePersonAsync_ShouldCreatePerson()
        {
            //Arrange
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

            Person expectedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            //Act
            _dataContext.People.Add(createdPerson);
            _dataContext.SaveChanges();

            //Assert
            createdPerson.Should().BeEquivalentTo(expectedPerson);
        }

        [Fact]
        public async void CreatePersonAsyncWithExistEmail_ShouldThrowException()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 2,
                GradeName = "Junior plus"
            };
            _dataContext.Grades.Add(testGrade);

            Person existPerson = new Person()
            {
                Email = "11112@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(existPerson);
            _dataContext.SaveChanges();

            Person personWithDuplicateEmail = new Person()
            {
                Email = "11112@test.com",
                FirstName = "John",
                LastName = "Bon",
                GradeId = testGrade.GradeId,
                ShortName = "Jovi",
                SurName = "Jovison",
                Grade = testGrade
            };

            // Act
            var result = async () => await _personService.CreatePersonAsync(personWithDuplicateEmail);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("A Person with same Email is already exists");
        }

        /// <summary>
        /// Проверяем что DeletePersonAsync действительно его сотрудника
        /// </summary>
        [Fact]
        public async void DeletePersonAsync_ShouldDeleteGrade()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior entry"
            };
            _dataContext.Grades.Add(testGrade);

            Person deletedPerson = new Person()
            {
                Email = "11112@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(deletedPerson);
            _dataContext.SaveChanges();

            //act
            await _personService.DeletePersonAsync(deletedPerson.PersonId);
            Person notExistPerson = _dataContext.People.Find(deletedPerson.PersonId);

            //assert
            notExistPerson.Should().BeNull();
        }

        /// <summary>
        /// Проверяем что при попытке удалить несуществующего сотрудника получим соответсвующую ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsPerson_ShouldThrowException()
        {
            //arrange
            Person notExistPerson = new Person
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = 1,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
            };            

            //act
            var result = async () => await _personService.DeletePersonAsync(notExistPerson.PersonId);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Person not found");
        }

        public void Dispose()
        {
            _dataContext.Database.EnsureDeleted();
        }
    }
}

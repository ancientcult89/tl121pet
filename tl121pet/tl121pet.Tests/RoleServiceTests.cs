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
    public class RoleServiceTests
    {
        private readonly DataContext _dataContext;
        private readonly IRoleService _roleService;
        public RoleServiceTests()
        {
            var connectionString = "Server=host.docker.internal;Database=TLRoleTest;Port=49153;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            _roleService = new RoleService(_dataContext);
        }

        /// <summary>
        /// Проверка, что GetRoleListAsync возвращает все доступные роли
        /// </summary>
        [Fact]
        public async void GetRoleListAsyncc_ShouldReturnAllRoles()
        {
            //Arrange
            _dataContext.Roles.AddRange((IEnumerable<Role>)RoleTestData.GetTestRoles());
            _dataContext.SaveChanges();

            //Act
            IEnumerable<Role> roles = await _roleService.GetRoleListAsync();
            IEnumerable<Role> resultRoles = (IEnumerable<Role>)RoleTestData.GetTestRoles();

            //Assert
            resultRoles.Should().BeEquivalentTo(roles);
        }

        /// <summary>
        /// Проверка, что GetRoleListAsync возвращает пустую коллекцию и не вываливается в ошибку, если нет ни одной записи
        /// </summary>
        [Fact]
        public async void GetRoleListAsync_ShouldReturnEmptyListIfRolesNotExists()
        {
            //Arrange
            List<Role> expectRoles = new();

            //Act
            IEnumerable<Role> grades = await _roleService.GetRoleListAsync();

            //Assert
            expectRoles.Should().BeEquivalentTo(grades);
        }


        /// <summary>
        /// Проверка, что GetGradeByIdAsync возвращает ожидаемую роль
        /// </summary>
        [Fact]
        public async void GetRoleByIdAsync_ShouldReturnCorrectRole()
        {
            //Arrange
            Role expectRole = RoleTestData.GetSingleRole();
            _dataContext.Roles.Add(expectRole);
            _dataContext.SaveChanges();

            //Act
            Role resultRole = await _roleService.GetRoleByIdAsync(expectRole.RoleId);

            //Assert
            resultRole.Should().BeEquivalentTo(expectRole);
        }

        /// <summary>
        /// Проверка, что GetRoleByIdAsync выкинет исключение с нужным текстом, если не найдено роли с таким ID
        /// </summary>
        [Fact]
        public async void GetRoleByIdAsync_ShouldThrowExceptionIfRoleNotExistsWithCorrectMessage()
        {
            // Act
            var result = async () => await _roleService.GetRoleByIdAsync(1);

            // Act & Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Role not found");
        }

        /// <summary>
        /// Тестируем метод CreateRoleAsync, проверяем, что создаётся корректная сущность
        /// </summary>
        [Fact]
        public async void CreateRoleAsync_ShouldCreateRole()
        {
            //Arrange
            Role expectRole = RoleTestData.GetSingleRole();
            await _roleService.CreateRoleAsync(expectRole);

            //Act
            Role resultRole = await _roleService.GetRoleByIdAsync(expectRole.RoleId);

            //Assert
            resultRole.Should().BeEquivalentTo(expectRole);
        }

        /// <summary>
        /// Проверка создания роли, убеждаемся, что при попытке внести роль с существующим именем получим ошибку
        /// </summary>
        [Fact]
        public async void CreateRoleAsync_CreatingDuplicateShouldThrowExeption()
        {
            //Arrange
            Role expectRole = RoleTestData.GetSingleRole();
            await _roleService.CreateRoleAsync(expectRole);
            Role expectrole2 = RoleTestData.GetAnotherSingleRole();

            // Act
            var result = async () => await _roleService.CreateRoleAsync(expectrole2);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("A Role with this name exists");
        }

        /// <summary>
        /// Положительная проверка изменения роли
        /// </summary>
        [Fact]
        public async void UpdateRoleAsync_ShouldChangeRoleName()
        {
            //Arrange
            Role testRole = RoleTestData.GetTestRole();
            await _roleService.CreateRoleAsync(testRole);
            Role expectRole = new Role
            {
                RoleId = testRole.RoleId,
                RoleName = "Testing Name",
            };

            // Act
            testRole.RoleName = "Testing Name";
            await _roleService.UpdateRoleAsync(testRole);

            // Assert
            testRole.Should().BeEquivalentTo(expectRole);
        }

        /// <summary>
        /// Проверка обновления роли, убеждаемся, что при попытке внести имя, которое уже существует в другой записи получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateRoleAsync_CreatingDuplicateShouldThrowExeption()
        {
            //Arrange
            Role expectRole = RoleTestData.GetTestRole();
            await _roleService.CreateRoleAsync(expectRole);
            Role expectRole2 = new Role
            {
                RoleId = 3,
                RoleName = "Test2",
            };
            await _roleService.CreateRoleAsync(expectRole2);

            expectRole2.RoleName = "Test";

            // Act
            var result = async () => await _roleService.UpdateRoleAsync(expectRole2);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("A Role with this name exists");
        }

        /// <summary>
        /// Убеждаемся что при попытке обновить несуществующую роль мы получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistsRole_ShouldThrowException()
        {
            //Arrange
            Role testGrade = RoleTestData.GetTestRole();

            //Act
            var result = async () => await _roleService.UpdateRoleAsync(testGrade);

            //Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Role not found");
        }

        /// <summary>
        /// Проверяем что удаление роли действительно её удаляет
        /// </summary>
        [Fact]
        public async void DeleteRoleAsync_ShouldDeleteRole()
        {
            //arrange
            Role role = RoleTestData.GetTestRole();
            _dataContext.Roles.Add(role);
            _dataContext.SaveChanges();

            //act
            await _roleService.DeleteRoleAsync(role.RoleId);
            Role notExistRole = _dataContext.Roles.Find(role.RoleId);

            //assert
            notExistRole.Should().BeNull();
        }

        /// <summary>
        /// Проверяем что при попытке удалить несуществующую роль получим соответсвующую ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsRole_ShouldThrowException()
        {
            //arrange
            Role role = RoleTestData.GetTestRole();

            //act
            var result = async () => await _roleService.DeleteRoleAsync(role.RoleId);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Role not found");
        }

        /// <summary>
        /// Проверяем что при попытке удалить админску роль получим соответсвующую ошибку
        /// </summary>
        [Fact]
        public async void DeleteAdminRole_ShouldThrowException()
        {
            //arrange
            Role role = RoleTestData.GetSingleRole();
            _dataContext.Roles.Add(role);
            _dataContext.SaveChanges();

            //act
            var result = async () => await _roleService.DeleteRoleAsync(role.RoleId);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Role \"Admin\" is system Role. You can not change this Role or Create It manually");
        }

        /// <summary>
        /// Проверка обновления роли, убеждаемся, что при попытке изменить имя Admin получим ошибку, т.к. это системная роль
        /// </summary>
        [Fact]
        public async void UpdateAdminRole_ShouldThrowExeption()
        {
            //Arrange
            Role expectRole = RoleTestData.GetSingleRole();
            await _roleService.CreateRoleAsync(expectRole);

            // Act
            expectRole.RoleName = "Test";
            var result = async () => await _roleService.UpdateRoleAsync(expectRole);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Role \"Admin\" is system Role. You can not change this Role or Create It manually");
        }
    }
}

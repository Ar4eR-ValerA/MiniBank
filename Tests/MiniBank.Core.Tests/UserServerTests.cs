using System;
using System.Collections.Generic;
using System.Threading;
using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Domain.Users.Services;
using MiniBank.Core.Tools;
using Moq;
using Xunit;

namespace MiniBank.Core.Tests;

public class UserServerTests
{
    private readonly IUserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;

    public UserServerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        var userValidatorMock = new Mock<IValidator<User>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _accountRepositoryMock.Object,
            userValidatorMock.Object,
            unitOfWorkMock.Object);
    }

    [Fact]
    public async void GetById_SuccessPath_UserReturned()
    {
        var expectedId = Guid.NewGuid();
        var expectedLogin = "Login";
        var expectedEmail = "Email";

        // ARRANGE
        var returnedUser = new User
        {
            Id = expectedId,
            Login = expectedLogin,
            Email = expectedEmail
        };

        _userRepositoryMock
            .Setup(userRepository =>
                userRepository.GetById(It.Is<Guid>(id => id == expectedId), CancellationToken.None))
            .ReturnsAsync(returnedUser);

        // ACT
        var user = await _userService.GetById(expectedId, CancellationToken.None);

        // ASSERT
        Assert.Equal(returnedUser, user);
        Assert.Equal(expectedId, user.Id);
        Assert.Equal(expectedLogin, user.Login);
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public async void GetAll_SuccessPath_UsersReturned()
    {
        // ARRANGE
        var expectedId = Guid.NewGuid();
        var expectedLogin = "Login";
        var expectedEmail = "Email";

        IReadOnlyList<User> returnedUsers = new List<User>
        {
            new()
            {
                Id = expectedId,
                Login = expectedLogin,
                Email = expectedEmail
            }
        };

        _userRepositoryMock
            .Setup(userRepository => userRepository.GetAll(CancellationToken.None))
            .ReturnsAsync(returnedUsers);

        // ACT
        var users = await _userService.GetAll(CancellationToken.None);

        // ASSERT
        Assert.Equal(returnedUsers, users);
        Assert.Equal(returnedUsers.Count, users.Count);
        Assert.Equal(expectedId, users[0].Id);
        Assert.Equal(expectedLogin, users[0].Login);
        Assert.Equal(expectedEmail, users[0].Email);
    }

    [Fact]
    public async void Create_SuccessPath_ReturnUserId()
    {
        // ARRANGE
        var expectedLogin = "Login";
        var expectedEmail = "Email";

        var user = new User
        {
            Login = expectedLogin,
            Email = expectedEmail
        };

        // ACT
        var userId = await _userService.Create(user, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, userId);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(expectedLogin, user.Login);
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void Create_DuplicateLogin_ThrowUserFriendlyException()
    {
        // ARRANGE
        var user = new User();

        _userRepositoryMock
            .Setup(userRepository => userRepository.IsLoginExists(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(true);

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Create(user, CancellationToken.None);
        });
    }

    [Fact]
    public async void Update_SuccessPath_NotThrow()
    {
        // ARRANGE
        var user = new User();

        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);

        // ACT, ASSERT
        await _userService.Update(user, CancellationToken.None);
        // TODO: Проверить UnitOfWork
    }

    [Fact]
    public void Update_NoSuchUser_ThrowUserFriendlyException()
    {
        // ARRANGE
        var user = new User();

        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(false);

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Update(user, CancellationToken.None);
        });
        // TODO: Проверить UnitOfWork
    }

    [Fact]
    public async void Delete_SuccessPath_NotThrow()
    {
        // ARRANGE
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(false);

        // ACT, ASSERT
        await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
    }

    [Fact]
    public void Delete_NoSuchUser_ThrowUserFriendlyException()
    {
        // ARRANGE
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(false);

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Fact]
    public void Delete_HasLinkedAccounts_ThrowUserFriendlyException()
    {
        // ARRANGE
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
        });
    }
}
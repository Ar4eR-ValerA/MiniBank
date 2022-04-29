using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        // ARRANGE
        var expectedUser = new User();
        
        _userRepositoryMock
            .Setup(userRepository => userRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(expectedUser));

        // ACT
        var user = await _userService.GetById(Guid.NewGuid(), CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedUser, user);
    }

    [Fact]
    public async void GetAll_SuccessPath_UsersReturned()
    {
        // ARRANGE
        IReadOnlyList<User> expectedUsers = new List<User>();
        
        _userRepositoryMock
            .Setup(userRepository => userRepository.GetAll(CancellationToken.None))
            .Returns(Task.FromResult(expectedUsers));

        // ACT
        var users = await _userService.GetAll(CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedUsers, users);
    }

    [Fact]
    public async void Create_SuccessPath_ReturnUserId()
    {
        // ARRANGE
        var user = new User();
        
        // ACT
        var userId = await _userService.Create(user, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public void Create_DuplicateLogin_ThrowUserFriendlyException()
    {
        // ARRANGE
        var user = new User();
        
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsLoginExists(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

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
            .Returns(Task.FromResult(true));

        // ACT, ASSERT
        await _userService.Update(user, CancellationToken.None);
    }

    [Fact]
    public void Update_NoSuchUser_ThrowUserFriendlyException()
    {
        // ARRANGE
        var user = new User();
        
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Update(user, CancellationToken.None);
        });
    }

    [Fact]
    public async void Delete_SuccessPath_NotThrow()
    {
        // ARRANGE
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        // ACT, ASSERT
        await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
    }

    [Fact]
    public void Delete_NoSuchUser_ThrowUserFriendlyException()
    {
        // ARRANGE
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

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
            .Returns(Task.FromResult(true));
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        // ACT, ASSERT
        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
        });
    }
}
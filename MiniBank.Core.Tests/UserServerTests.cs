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
    private readonly Mock<IUserRepository> _fakeUserRepository;
    private readonly Mock<IAccountRepository> _fakeAccountRepository;

    public UserServerTests()
    {
        _fakeUserRepository = new Mock<IUserRepository>();
        _fakeAccountRepository = new Mock<IAccountRepository>();
        var fakeUserValidator = new Mock<IValidator<User>>();
        var fakeUnitOfWork = new Mock<IUnitOfWork>();

        _userService = new UserService(
            _fakeUserRepository.Object,
            _fakeAccountRepository.Object,
            fakeUserValidator.Object,
            fakeUnitOfWork.Object);
    }

    [Fact]
    public async void GetById_SuccessPath_ReturnedUser()
    {
        var expectedUser = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(expectedUser));

        var user = await _userService.GetById(Guid.NewGuid());

        Assert.Equal(expectedUser, user);
    }

    [Fact]
    public async void GetAll_SuccessPath_ReturnedUsers()
    {
        IReadOnlyList<User> expectedUsers = new List<User>();

        _fakeUserRepository
            .Setup(userRepository => userRepository.GetAll(CancellationToken.None))
            .Returns(Task.FromResult(expectedUsers));

        var users = await _userService.GetAll();

        Assert.Equal(expectedUsers, users);
    }

    [Fact]
    public async void AddUser_SuccessPath_ReturnUserId()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.Create(It.IsAny<User>(), CancellationToken.None));

        var userId = await _userService.Create(user);

        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public void AddUser_DuplicateLogin_ThrowUserFriendlyException()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsLoginExists(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        Assert.ThrowsAsync<UserFriendlyException>(async () => { await _userService.Create(user); });
    }

    [Fact]
    public async void UpdateUser_SuccessPath_NotThrow()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        await _userService.Update(user);
    }
    
    [Fact]
    public void UpdateUser_NoSuchUser_ThrowUserFriendlyException()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        Assert.ThrowsAsync<UserFriendlyException>(async () => { await _userService.Update(user); });
    }

    [Fact]
    public async void DeleteUser_SuccessPath_NotThrow()
    {
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));
        
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        await _userService.Delete(Guid.NewGuid());;
    }
    
    [Fact]
    public void DeleteUser_NoSuchUser_ThrowUserFriendlyException()
    {
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        Assert.ThrowsAsync<UserFriendlyException>(async () => { await _userService.Delete(Guid.NewGuid()); });
    }

    [Fact]
    public void DeleteUser_HasLinkedAccounts_ThrowUserFriendlyException()
    {
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        Assert.ThrowsAsync<UserFriendlyException>(async () => { await _userService.Delete(Guid.NewGuid()); });
    }
}
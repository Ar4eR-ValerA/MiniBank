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
    public async void GetById_SuccessPath_UserReturned()
    {
        var expectedUser = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(expectedUser));

        var user = await _userService.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(expectedUser, user);
    }

    [Fact]
    public async void GetAll_SuccessPath_UsersReturned()
    {
        IReadOnlyList<User> expectedUsers = new List<User>();

        _fakeUserRepository
            .Setup(userRepository => userRepository.GetAll(CancellationToken.None))
            .Returns(Task.FromResult(expectedUsers));

        var users = await _userService.GetAll(CancellationToken.None);

        Assert.Equal(expectedUsers, users);
    }

    [Fact]
    public async void Create_SuccessPath_ReturnUserId()
    {
        var user = new User();

        //TODO: Мб не мокать валидатор и нормально его тестировать?
        var userId = await _userService.Create(user, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public void Create_DuplicateLogin_ThrowUserFriendlyException()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsLoginExists(It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Create(user, CancellationToken.None);
        });
    }

    [Fact]
    public async void Update_SuccessPath_NotThrow()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        await _userService.Update(user, CancellationToken.None);
    }

    [Fact]
    public void Update_NoSuchUser_ThrowUserFriendlyException()
    {
        var user = new User();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Update(user, CancellationToken.None);
        });
    }

    [Fact]
    public async void Delete_SuccessPath_NotThrow()
    {
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
    }

    [Fact]
    public void Delete_NoSuchUser_ThrowUserFriendlyException()
    {
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Fact]
    public void Delete_HasLinkedAccounts_ThrowUserFriendlyException()
    {
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.HasUserLinkedAccounts(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _userService.Delete(Guid.NewGuid(), CancellationToken.None);
        });
    }
}
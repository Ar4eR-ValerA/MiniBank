using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Domain.Users.Services;
using Moq;
using Xunit;

namespace MiniBank.Core.Tests;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly Mock<IUserRepository> _fakeUserRepository;
    private readonly Mock<IAccountRepository> _fakeAccountRepository;
    private readonly Mock<IValidator<User>> _userValidator;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public UserServiceTests()
    {
        _fakeUserRepository = new Mock<IUserRepository>();
        _fakeAccountRepository = new Mock<IAccountRepository>();
        _userValidator = new Mock<IValidator<User>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _userService = new UserService(
            _fakeUserRepository.Object, 
            _fakeAccountRepository.Object, 
            _userValidator.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public void test1()
    {
        _fakeUserRepository.Setup(userRepository => userRepository.Create(It.IsAny<User>()));
    }
}
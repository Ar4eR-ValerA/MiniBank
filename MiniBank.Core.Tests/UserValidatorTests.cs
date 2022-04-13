using System;
using FluentValidation;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Validators;
using Xunit;

namespace MiniBank.Core.Tests;

public class UserValidatorTests
{
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _validator = new UserValidator();
    }

    [Theory]
    [InlineData("TestLogin", "TestEmail")]
    public void Validate_SuccessPath_NotThrow(string login, string email)
    {
        var user = new User { Login = login, Email = email, Id = Guid.NewGuid()};

        _validator.ValidateAndThrow(user);
    }

    [Fact]
    public void Validate_EmptyLogin_ThrowValidationException()
    {
        var user = new User { Login = string.Empty };

        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(user));
    }
    
    [Theory]
    [InlineData("qwertyuityuooppasdfdfgnghvdaferwget")]
    public void Validate_Over20LengthLogin_ThrowValidationException(string login)
    {
        var user = new User { Login = login };

        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(user));
    }
}
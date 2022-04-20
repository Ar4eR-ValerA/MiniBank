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
        // ARRANGE
        var user = new User { Login = login, Email = email, Id = Guid.NewGuid() };

        // ACT, ASSERT
        _validator.ValidateAndThrow(user);
    }

    [Fact]
    public void Validate_EmptyLogin_ThrowValidationException()
    {
        // ARRANGE
        var user = new User { Login = string.Empty };

        // ACT, ASSERT
        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(user));
    }

    [Theory]
    [InlineData("string with length greater than 20")]
    public void Validate_Over20LengthLogin_ThrowValidationException(string login)
    {
        // ARRANGE
        var user = new User { Login = login };

        // ACT, ASSERT
        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(user));
    }
}
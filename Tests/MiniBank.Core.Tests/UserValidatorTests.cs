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

    [Fact]
    public void Validate_SuccessPath_NotThrow()
    {
        // ARRANGE
        const string testLogin = "TestLogin";
        const string testEmail = "TestEmail";

        var user = new User { Login = testLogin, Email = testEmail, Id = Guid.NewGuid() };

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

    [Fact]
    public void Validate_Over20LengthLogin_ThrowValidationException()
    {
        // ARRANGE
        const string longString = "string with length greater than 20";

        var user = new User { Login = longString };

        // ACT, ASSERT
        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(user));
    }
}
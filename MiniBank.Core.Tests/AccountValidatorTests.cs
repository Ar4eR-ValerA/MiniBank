using System;
using FluentValidation;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Validators;
using MiniBank.Core.Domain.Currencies;
using Xunit;

namespace MiniBank.Core.Tests;

public class AccountValidatorTests
{
    private readonly AccountValidator _validator;

    public AccountValidatorTests()
    {
        _validator = new AccountValidator();
    }

    [Theory]
    [InlineData(100, Currency.RUB, true)]
    public void Validate_SuccessPath_NotThrow(double balance, Currency currency, bool isActive)
    {
        // ARRANGE
        var account = new Account
        {
            Balance = balance, 
            Currency = currency, 
            DateOpened = DateTime.UtcNow, 
            DateClosed = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsActive = isActive, 
            UserId = Guid.NewGuid()
        };

        // ACT, ASSERT
        _validator.ValidateAndThrow(account);
    }

    [Theory]
    [InlineData(-100)]
    public void Validate_NegativeBalance_ThrowValidationException(double balance)
    {
        // ARRANGE
        var account = new Account { Balance = balance };

        // ASSERT
        Assert.Throws<ValidationException>(() =>
        {
            // ACT
            _validator.ValidateAndThrow(account);
        });
    }
}
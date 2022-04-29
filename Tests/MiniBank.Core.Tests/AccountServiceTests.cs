using System;
using System.Collections.Generic;
using System.Threading;
using FluentValidation;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Core.Domain.Currencies;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;
using Moq;
using Xunit;

namespace MiniBank.Core.Tests;

public class AccountServiceTests
{
    private readonly IAccountService _accountService;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICurrencyRateConversionService> _currencyRateConversionServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AccountServiceTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        var transactionRepositoryMock = new Mock<ITransactionRepository>();
        _currencyRateConversionServiceMock = new Mock<ICurrencyRateConversionService>();
        var accountValidatorMock = new Mock<IValidator<Account>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _accountService = new AccountService(
            _accountRepositoryMock.Object,
            _currencyRateConversionServiceMock.Object,
            transactionRepositoryMock.Object,
            accountValidatorMock.Object,
            unitOfWorkMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async void GetById_SuccessPath_AccountReturned()
    {
        // ARRANGE
        var expectedAccount = new Account();
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(expectedAccount);

        // ACT
        var account = await _accountService.GetById(Guid.NewGuid(), CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedAccount, account);
    }

    [Fact]
    public async void GetAll_SuccessPath_AccountsReturned()
    {
        // ARRANGE
        IReadOnlyList<Account> expectedAccounts = new List<Account>();
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetAll(CancellationToken.None))
            .ReturnsAsync(expectedAccounts);

        // ACT
        var accounts = await _accountService.GetAll(CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedAccounts, accounts);
    }

    [Fact]
    public async void Create_SuccessPath_CorrectAccountCreated()
    {
        // ARRANGE
        var account = new Account();
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);

        // ACT
        var accountId = await _accountService.Create(account, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, accountId);
        Assert.Equal(DateTime.UtcNow.Date, account.DateOpened.Date);
        Assert.True(account.IsActive);
    }

    [Fact]
    public async void Create_NotSuchUser_ThrowUserFriendlyException()
    {
        // ARRANGE
        var account = new Account();
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(false);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Create(account, CancellationToken.None);
        });
    }

    [Fact]
    public async void Close_SuccessPath_AccountClosed()
    {
        // ARRANGE
        var returnedAccount = new Account { IsActive = true, Balance = 0 };
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(returnedAccount);

        // ACT
        await _accountService.Close(Guid.NewGuid(), CancellationToken.None);

        // ASSERT
        Assert.Equal(DateTime.UtcNow.Date, returnedAccount.DateClosed?.Date);
        Assert.False(returnedAccount.IsActive);
    }

    [Fact]
    public async void Close_NotActive_ThrowUserFriendlyException()
    {
        // ARRANGE
        var returnedAccount = new Account { IsActive = false };
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(returnedAccount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Close(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Fact]
    public async void Close_NotZeroBalance_ThrowUserFriendlyException()
    {
        // ARRANGE
        const double positiveBalance = 100;

        var returnedAccount = new Account { IsActive = true, Balance = positiveBalance };

        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(returnedAccount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Close(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Fact]
    public async void CalculateCommission_SuccessPathDifferentUsers_NotZeroCommission()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var userFromId = Guid.NewGuid();
        var userToId = Guid.NewGuid();

        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountFromId,
                UserId = userFromId
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountToId,
                UserId = userToId
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT
        var commission = await _accountService.CalculateCommission(
            positiveAmount,
            returnedAccountFromId,
            returnedAccountToId,
            CancellationToken.None);

        // ASSERT
        Assert.NotEqual(0, commission);
    }

    [Fact]
    public async void CalculateCommission_SuccessPathSameUser_ZeroCommission()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var userId = Guid.NewGuid();

        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountFromId,
                UserId = userId
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountToId,
                UserId = userId
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT
        var commission = await _accountService.CalculateCommission(
            positiveAmount,
            returnedAccountFromId,
            returnedAccountToId,
            CancellationToken.None);

        // ASSERT
        Assert.Equal(0, commission);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    public async void CalculateCommission_InvalidAmount_ThrowUserFriendlyException(double notPositiveAmount)
    {
        // ARRANGE
        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountFromId,
                UserId = Guid.NewGuid()
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = returnedAccountToId,
                UserId = Guid.NewGuid()
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(notPositiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.CalculateCommission(
                notPositiveAmount,
                returnedAccountFromId,
                returnedAccountToId,
                CancellationToken.None);
        });
    }

    [Fact]
    public async void MakeTransaction_SuccessPath_TransactionMade()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var accountToId = Guid.NewGuid();
        var accountFromId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = positiveAmount
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid()
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT
        var transactionId = await _accountService
            .MakeTransaction(positiveAmount, accountFromId, accountToId, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, transactionId);
    }

    [Theory]
    [InlineData(-20)]
    [InlineData(0)]
    public async void MakeTransaction_InvalidAmount_ThrowUserFriendlyException(double notPositiveAmount)
    {
        // ARRANGE
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = notPositiveAmount
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid(),
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(notPositiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(notPositiveAmount, accountFromId, accountToId,
                CancellationToken.None);
        });
    }

    [Fact]
    public async void MakeTransaction_SameAccount_ThrowUserFriendlyException()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountId,
                UserId = userId,
                IsActive = true,
                Balance = positiveAmount
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(positiveAmount, accountId, accountId, CancellationToken.None);
        });
    }

    [Fact]
    public async void MakeTransaction_SenderIsNotActive_ThrowUserFriendlyException()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = false,
                Balance = positiveAmount
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid(),
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(positiveAmount, accountFromId, accountToId,
                CancellationToken.None);
        });
    }

    [Fact]
    public async void MakeTransaction_ReceiverIsNotActive_ThrowUserFriendlyException()
    {
        // ARRANGE
        const double positiveAmount = 100;

        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = positiveAmount
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = false,
                UserId = Guid.NewGuid(),
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(positiveAmount, accountFromId, accountToId,
                CancellationToken.None);
        });
    }

    [Fact]
    public async void MakeTransaction_NegativeBalanceAfterTransaction_ThrowUserFriendlyException()
    {
        // ARRANGE
        const double positiveAmount = 100;
        const double positiveBalanceFrom = 20;

        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = positiveBalanceFrom
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = false,
                UserId = Guid.NewGuid(),
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        // ACT, ASSERT
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(positiveAmount, accountFromId, accountToId,
                CancellationToken.None);
        });
    }
}
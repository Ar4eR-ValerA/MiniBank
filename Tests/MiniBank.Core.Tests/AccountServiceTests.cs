using System;
using System.Collections.Generic;
using System.Threading;
using FluentValidation;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Core.Domain.Currencies;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Transactions;
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
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AccountServiceTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _currencyRateConversionServiceMock = new Mock<ICurrencyRateConversionService>();
        var accountValidatorMock = new Mock<IValidator<Account>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _accountService = new AccountService(
            _accountRepositoryMock.Object,
            _currencyRateConversionServiceMock.Object,
            _transactionRepositoryMock.Object,
            accountValidatorMock.Object,
            unitOfWorkMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async void GetById_SuccessPath_AccountReturned()
    {
        // ARRANGE
        var expectedId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedDateOpened = DateTime.UtcNow;
        var expectedBalance = 100;
        var expectedCurrency = Currency.EUR;
        var expectedIsActive = true;
        var expectedDateClosed = DateTime.UtcNow;

        var returnedAccount = new Account
        {
            Id = expectedId,
            UserId = expectedUserId,
            DateOpened = expectedDateOpened,
            Balance = expectedBalance,
            Currency = expectedCurrency,
            IsActive = expectedIsActive,
            DateClosed = expectedDateClosed
        };
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == expectedId), CancellationToken.None))
            .ReturnsAsync(returnedAccount);

        // ACT
        var account = await _accountService.GetById(expectedId, CancellationToken.None);

        // ASSERT
        Assert.Equal(returnedAccount, account);
        Assert.Equal(expectedId, account.Id);
        Assert.Equal(expectedUserId, account.UserId);
        Assert.Equal(expectedDateOpened, account.DateOpened);
        Assert.Equal(expectedBalance, account.Balance);
        Assert.Equal(expectedCurrency, account.Currency);
        Assert.Equal(expectedIsActive, account.IsActive);
        Assert.Equal(expectedDateClosed, account.DateClosed);
    }

    [Fact]
    public async void GetAll_SuccessPath_AccountsReturned()
    {
        // ARRANGE
        var expectedId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedDateOpened = DateTime.UtcNow;
        var expectedBalance = 100;
        var expectedCurrency = Currency.EUR;
        var expectedIsActive = true;
        var expectedDateClosed = DateTime.UtcNow;

        IReadOnlyList<Account> returnedAccounts = new List<Account>
        {
            new()
            {
                Id = expectedId,
                UserId = expectedUserId,
                DateOpened = expectedDateOpened,
                Balance = expectedBalance,
                Currency = expectedCurrency,
                IsActive = expectedIsActive,
                DateClosed = expectedDateClosed
            }
        };
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetAll(CancellationToken.None))
            .ReturnsAsync(returnedAccounts);

        // ACT
        var accounts = await _accountService.GetAll(CancellationToken.None);

        // ASSERT
        Assert.Equal(returnedAccounts, accounts);
        Assert.Equal(returnedAccounts.Count, accounts.Count);
        Assert.Equal(expectedId, accounts[0].Id);
        Assert.Equal(expectedUserId, accounts[0].UserId);
        Assert.Equal(expectedDateOpened, accounts[0].DateOpened);
        Assert.Equal(expectedBalance, accounts[0].Balance);
        Assert.Equal(expectedCurrency, accounts[0].Currency);
        Assert.Equal(expectedIsActive, accounts[0].IsActive);
        Assert.Equal(expectedDateClosed, accounts[0].DateClosed);
    }

    [Fact]
    public async void Create_SuccessPath_CorrectAccountCreated()
    {
        // ARRANGE
        var expectedUserId = Guid.NewGuid();
        var expectedDateOpened = DateTime.UtcNow.Date;
        var expectedBalance = 100;
        var expectedCurrency = Currency.EUR;
        var expectedIsActive = true;
        DateTime? expectedDateClosed = null;

        var returnedAccount = new Account
        {
            UserId = expectedUserId,
            Balance = expectedBalance,
            Currency = expectedCurrency,
        };
        _userRepositoryMock
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(true);

        // ACT
        var accountId = await _accountService.Create(returnedAccount, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, returnedAccount.Id);
        Assert.NotEqual(Guid.Empty, accountId);
        Assert.Equal(expectedUserId, returnedAccount.UserId);
        Assert.Equal(expectedDateOpened, returnedAccount.DateOpened.Date);
        Assert.Equal(expectedBalance, returnedAccount.Balance);
        Assert.Equal(expectedCurrency, returnedAccount.Currency);
        Assert.Equal(expectedIsActive, returnedAccount.IsActive);
        Assert.Equal(expectedDateClosed, returnedAccount.DateClosed);
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
        var expectedId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedDateOpened = DateTime.UtcNow.Date;
        var expectedBalance = 0;
        var expectedCurrency = Currency.USD;
        var returnedIsActive = true;
        var expectedIsActive = false;
        var expectedDateClosed = DateTime.UtcNow.Date;

        var returnedAccount = new Account
        {
            Id = expectedId,
            UserId = expectedUserId,
            DateOpened = expectedDateOpened,
            Balance = expectedBalance,
            Currency = expectedCurrency,
            IsActive = returnedIsActive
        };
        _accountRepositoryMock
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(returnedAccount);

        // ACT
        await _accountService.Close(Guid.NewGuid(), CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedId, returnedAccount.Id);
        Assert.Equal(expectedUserId, returnedAccount.UserId);
        Assert.Equal(expectedDateOpened, returnedAccount.DateOpened);
        Assert.Equal(expectedBalance, returnedAccount.Balance);
        Assert.Equal(expectedCurrency, returnedAccount.Currency);
        Assert.Equal(expectedIsActive, returnedAccount.IsActive);
        Assert.Equal(expectedDateClosed, returnedAccount.DateClosed?.Date);
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
        double positiveAmount = 100;
        var expectedCommission = Math.Round(positiveAmount * 0.02, 2);

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
        Assert.Equal(expectedCommission, commission);
    }

    [Fact]
    public async void CalculateCommission_SuccessPathSameUser_ZeroCommission()
    {
        // ARRANGE
        double positiveAmount = 100;

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

    [Theory]
    [InlineData(100, 100)]
    [InlineData(20, 100)]
    public async void MakeTransaction_SuccessPath_TransactionMade(double positiveAmount, double positiveBalanceFrom)
    {
        // ARRANGE
        var expectedCurrency = Currency.EUR;
        var expectedCommission = Math.Round(positiveAmount * 0.02, 2);
        var expectedAmount = positiveAmount - expectedCommission;

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
                Currency = expectedCurrency,
                Balance = positiveBalanceFrom
            });
        _accountRepositoryMock
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .ReturnsAsync(new Account
            {
                Id = accountToId,
                IsActive = true,
                Currency = expectedCurrency,
                UserId = Guid.NewGuid()
            });

        _currencyRateConversionServiceMock
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .ReturnsAsync(positiveAmount);

        _transactionRepositoryMock.Setup(transactionRepository =>
            transactionRepository.Create(It.IsAny<Transaction>(), CancellationToken.None));

        // ACT
        var transactionId = await _accountService
            .MakeTransaction(positiveAmount, accountFromId, accountToId, CancellationToken.None);

        // ASSERT
        _transactionRepositoryMock.Verify(transactionRepository =>
            transactionRepository.Create(It.Is<Transaction>(transaction =>
                Math.Abs(transaction.Amount - expectedAmount) < 0.001 &&
                transaction.Currency == expectedCurrency &&
                Math.Abs(transaction.Commission - expectedCommission) < 0.001 &&
                transaction.FromAccountId == accountFromId &&
                transaction.ToAccountId == accountToId &&
                transaction.Id != Guid.Empty &&
                transactionId == transaction.Id), It.IsAny<CancellationToken>()));
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

    [Theory]
    [InlineData(100, 20)]
    [InlineData(100, 100)]
    public async void MakeTransaction_NegativeBalanceAfterTransaction_ThrowUserFriendlyException(
        double positiveAmount, 
        double positiveBalanceFrom)
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
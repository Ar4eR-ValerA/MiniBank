using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly Mock<IAccountRepository> _fakeAccountRepository;
    private readonly Mock<ICurrencyRateConversionService> _fakeCurrencyRateConversionService;
    private readonly Mock<IUserRepository> _fakeUserRepository;

    public AccountServiceTests()
    {
        _fakeAccountRepository = new Mock<IAccountRepository>();
        var fakeTransactionRepository = new Mock<ITransactionRepository>();
        _fakeCurrencyRateConversionService = new Mock<ICurrencyRateConversionService>();
        var fakeAccountValidator = new Mock<IValidator<Account>>();
        var fakeUnitOfWork = new Mock<IUnitOfWork>();
        _fakeUserRepository = new Mock<IUserRepository>();

        _accountService = new AccountService(
            _fakeAccountRepository.Object,
            _fakeCurrencyRateConversionService.Object,
            fakeTransactionRepository.Object,
            fakeAccountValidator.Object,
            fakeUnitOfWork.Object,
            _fakeUserRepository.Object);
    }

    [Fact]
    public async void GetById_SuccessPath_AccountReturned()
    {
        var expectedAccount = new Account();
        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(expectedAccount));

        var account = await _accountService.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(expectedAccount, account);
    }

    [Fact]
    public async void GetAll_SuccessPath_AccountsReturned()
    {
        IReadOnlyList<Account> expectedAccounts = new List<Account>();

        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetAll(CancellationToken.None))
            .Returns(Task.FromResult(expectedAccounts));

        var accounts = await _accountService.GetAll(CancellationToken.None);

        Assert.Equal(expectedAccounts, accounts);
    }

    [Fact]
    public async void Create_SuccessPath_CorrectAccountCreated()
    {
        var account = new Account();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        var accountId = await _accountService.Create(account, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, accountId);
        Assert.Equal(DateTime.UtcNow.Date, account.DateOpened.Date);
        Assert.True(account.IsActive);
    }

    [Fact]
    public async void Create_NotSuchUser_ThrowUserFriendlyException()
    {
        var account = new Account();
        _fakeUserRepository
            .Setup(userRepository => userRepository.IsExist(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Create(account, CancellationToken.None);
        });
    }

    [Fact]
    public async void Close_SuccessPath_AccountClosed()
    {
        var returnedAccount = new Account { IsActive = true, Balance = 0 };
        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccount));

        await _accountService.Close(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(DateTime.UtcNow.Date, returnedAccount.DateClosed?.Date);
        Assert.False(returnedAccount.IsActive);
    }

    [Fact]
    public async void Close_NotActive_ThrowUserFriendlyException()
    {
        var returnedAccount = new Account { IsActive = false };
        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Close(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void Close_NotZeroBalance_ThrowUserFriendlyException(double balance)
    {
        var returnedAccount = new Account { IsActive = true, Balance = balance };

        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Close(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void CalculateCommission_SuccessPathDifferentUsers_NotZeroCommission(double amount)
    {
        var userFromId = Guid.NewGuid();
        var userToId = Guid.NewGuid();

        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountFromId,
                UserId = userFromId
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountToId,
                UserId = userToId
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        var commission = await _accountService.CalculateCommission(
            amount,
            returnedAccountFromId,
            returnedAccountToId,
            CancellationToken.None);

        Assert.NotEqual(0, commission);
    }

    [Theory]
    [InlineData(100)]
    public async void CalculateCommission_SuccessPathSameUser_ZeroCommission(double amount)
    {
        var userId = Guid.NewGuid();

        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountFromId,
                UserId = userId
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountToId,
                UserId = userId
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        var commission = await _accountService.CalculateCommission(
            amount,
            returnedAccountFromId,
            returnedAccountToId,
            CancellationToken.None);

        Assert.Equal(0, commission);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    public async void CalculateCommission_InvalidAmount_ThrowUserFriendlyException(double amount)
    {
        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountFromId,
                UserId = Guid.NewGuid()
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = returnedAccountToId,
                UserId = Guid.NewGuid()
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.CalculateCommission(
                amount,
                returnedAccountFromId,
                returnedAccountToId,
                CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void MakeTransaction_SuccessPath_TransactionMade(double amount)
    {
        var accountToId = Guid.NewGuid();
        var accountFromId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = amount
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid()
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        var transactionId = await _accountService
            .MakeTransaction(amount, accountFromId, accountToId, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, transactionId);
    }

    [Theory]
    [InlineData(-20)]
    [InlineData(0)]
    public async void MakeTransaction_InvalidAmount_ThrowUserFriendlyException(double amount)
    {
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = amount
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid(),
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(amount, accountFromId, accountToId, CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void MakeTransaction_SameAccount_ThrowUserFriendlyException(double amount)
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountId,
                UserId = userId,
                IsActive = true,
                Balance = amount
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(amount, accountId, accountId, CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void MakeTransaction_SenderIsNotActive_ThrowUserFriendlyException(double amount)
    {
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = false,
                Balance = amount
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountToId,
                IsActive = true,
                UserId = Guid.NewGuid(),
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(amount, accountFromId, accountToId, CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100)]
    public async void MakeTransaction_ReceiverIsNotActive_ThrowUserFriendlyException(double amount)
    {
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = amount
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountToId,
                IsActive = false,
                UserId = Guid.NewGuid(),
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(amount, accountFromId, accountToId, CancellationToken.None);
        });
    }

    [Theory]
    [InlineData(100, 20)]
    public async void MakeTransaction_NegativeBalanceAfterTransaction_ThrowUserFriendlyException(
        double amount,
        double balanceFrom)
    {
        var accountFromId = Guid.NewGuid();
        var accountToId = Guid.NewGuid();

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountFromId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountFromId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                Balance = balanceFrom
            }));
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == accountToId), CancellationToken.None))
            .Returns(Task.FromResult(new Account
            {
                Id = accountToId,
                IsActive = false,
                UserId = Guid.NewGuid(),
            }));
        _fakeCurrencyRateConversionService
            .Setup(currencyRateConversionService =>
                currencyRateConversionService.ConvertCurrencyRate(
                    It.IsAny<double>(), 
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>()))
            .Returns(Task.FromResult(amount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.MakeTransaction(amount, accountFromId, accountToId, CancellationToken.None);
        });
    }
}
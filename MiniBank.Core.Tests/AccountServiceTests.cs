using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Accounts.Services;
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
    private readonly Mock<ITransactionRepository> _fakeTransactionRepository;
    private readonly Mock<IUserRepository> _fakeUserRepository;

    public AccountServiceTests()
    {
        _fakeAccountRepository = new Mock<IAccountRepository>();
        _fakeTransactionRepository = new Mock<ITransactionRepository>();
        _fakeCurrencyRateConversionService = new Mock<ICurrencyRateConversionService>();
        var fakeAccountValidator = new Mock<IValidator<Account>>();
        var fakeUnitOfWork = new Mock<IUnitOfWork>();
        _fakeUserRepository = new Mock<IUserRepository>();

        _accountService = new AccountService(
            _fakeAccountRepository.Object,
            _fakeCurrencyRateConversionService.Object,
            _fakeTransactionRepository.Object,
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

    [Fact]
    public async void Close_NotZeroBalance_ThrowUserFriendlyException()
    {
        //TODO: Глянуть, как в лекции или мб спросить, оставлять ли магические числа такого типа
        var returnedAccount = new Account { IsActive = true, Balance = 100 };
        _fakeAccountRepository
            .Setup(accountRepository => accountRepository.GetById(It.IsAny<Guid>(), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccount));

        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _accountService.Close(Guid.NewGuid(), CancellationToken.None);
        });
    }

    [Fact]
    public async void CalculateCommission_SuccessPath_NotZeroCommission()
    {
        var returnedAccountFromId = Guid.NewGuid();
        var returnedAccountToId = Guid.NewGuid();
        var returnedAccountFrom = new Account { Id = returnedAccountFromId, UserId = Guid.NewGuid() };
        var returnedAccountTo = new Account { Id = returnedAccountToId, UserId = Guid.NewGuid() };
        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountFromId), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccountFrom));

        _fakeAccountRepository
            .Setup(accountRepository =>
                accountRepository.GetById(It.Is<Guid>(id => id == returnedAccountToId), CancellationToken.None))
            .Returns(Task.FromResult(returnedAccountTo));

        var commission = _accountService.CalculateCommission(
            new Random().Next(1, 2),
            //TODO: Чёт так себе идея
            returnedAccountFromId,
            returnedAccountToId,
            CancellationToken.None);
        
        Assert.True(await commission > 0);
    }
    
    //TODO: Calculate_Throw
    //TODO: MakeTransaction_Success
    //TODO: MakeTransaction_Throw
}
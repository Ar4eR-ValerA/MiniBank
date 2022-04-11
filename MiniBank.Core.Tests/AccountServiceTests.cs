using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Domain.Users.Repositories;
using Moq;
using Xunit;

namespace MiniBank.Core.Tests;

public class AccountServiceTests
{
    private readonly IAccountService _accountService;
    private readonly Mock<IAccountRepository> _fakeAccountRepository;
    private readonly Mock<ICurrencyRateConversionService> _fakeCurrencyRateConversionService;
    private readonly Mock<ITransactionRepository> _fakeTransactionRepository;
    private readonly Mock<IValidator<Account>> _fakeAccountValidator;
    private readonly Mock<IUnitOfWork> _fakeUnitOfWork;
    private readonly Mock<IUserRepository> _fakeUserRepository;

    public AccountServiceTests()
    {
        _fakeAccountRepository = new Mock<IAccountRepository>();
        _fakeTransactionRepository = new Mock<ITransactionRepository>();
        _fakeCurrencyRateConversionService = new Mock<ICurrencyRateConversionService>();
        _fakeAccountValidator = new Mock<IValidator<Account>>();
        _fakeUnitOfWork = new Mock<IUnitOfWork>();
        _fakeUserRepository = new Mock<IUserRepository>();
        
        _accountService = new AccountService(
            _fakeAccountRepository.Object, 
            _fakeCurrencyRateConversionService.Object,
            _fakeTransactionRepository.Object,
            _fakeAccountValidator.Object,
            _fakeUnitOfWork.Object,
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
}
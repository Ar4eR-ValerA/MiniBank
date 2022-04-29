using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Transactions;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Domain.Accounts.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRateConversionService _currencyRateConversionService;
    private readonly IValidator<Account> _accountValidator;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(
        IAccountRepository accountRepository,
        ICurrencyRateConversionService currencyRateConversionService,
        ITransactionRepository transactionRepository,
        IValidator<Account> accountValidator,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _accountValidator = accountValidator;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _currencyRateConversionService = currencyRateConversionService;
    }

    private double CalculateCommission(double amount, Account fromAccount, Account toAccount)
    {
        if (fromAccount.UserId == toAccount.UserId)
        {
            return 0;
        }

        double commission = Math.Round(amount * 0.02, 2);

        return commission;
    }

    private void ValidateTransactionAndThrow(double amount, Account fromAccount, Account toAccount)
    {
        if (fromAccount.Id == toAccount.Id)
        {
            throw new UserFriendlyException("Accounts must be different");
        }

        if (amount <= 0)
        {
            throw new UserFriendlyException("Transaction amount must be positive");
        }

        if (!fromAccount.IsActive)
        {
            throw new UserFriendlyException("Sender is not active");
        }

        if (!toAccount.IsActive)
        {
            throw new UserFriendlyException("Receiver is not active");
        }

        if (fromAccount.Balance - amount < 0)
        {
            throw new UserFriendlyException("Balance will be negative after operation");
        }
    }

    public Task<Account> GetById(Guid id, CancellationToken cancellationToken)
    {
        return _accountRepository.GetById(id, cancellationToken);
    }

    public Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken)
    {
        return _accountRepository.GetAll(cancellationToken);
    }

    public async Task<Guid> Create(Account account, CancellationToken cancellationToken)
    {
        await _accountValidator.ValidateAndThrowAsync(account, cancellationToken);

        if (!await _userRepository.IsExist(account.UserId, cancellationToken))
        {
            throw new UserFriendlyException($"There is no user with such id: {account.UserId}");
        }

        var accountId = Guid.NewGuid();
        account.Id = accountId;
        account.DateOpened = DateTime.UtcNow;
        account.IsActive = true;

        await _accountRepository.Create(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return accountId;
    }

    public async Task Close(Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetById(id, cancellationToken);

        if (!account.IsActive)
        {
            throw new UserFriendlyException("Account is already closed");
        }

        if (account.Balance != 0)
        {
            throw new UserFriendlyException("You can't close account with no zero balance");
        }

        account.IsActive = false;
        account.DateClosed = DateTime.UtcNow;

        await _accountRepository.Update(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<double> CalculateCommission(
        double amount, 
        Guid fromAccountId, 
        Guid toAccountId,
        CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            throw new UserFriendlyException("Amount must be positive");
        }

        var fromAccount = await _accountRepository.GetById(fromAccountId, cancellationToken);
        var toAccount = await _accountRepository.GetById(toAccountId, cancellationToken);

        if (fromAccount.UserId == toAccount.UserId)
        {
            return 0;
        }

        double commission = Math.Round(amount * 0.02, 2);

        return commission;
    }

    public async Task<Guid> MakeTransaction(
        double amount,
        Guid fromAccountId,
        Guid toAccountId,
        CancellationToken cancellationToken)
    {
        var fromAccount = await _accountRepository.GetById(fromAccountId, cancellationToken);
        var toAccount = await _accountRepository.GetById(toAccountId, cancellationToken);

        var commission = CalculateCommission(amount, fromAccount, toAccount);
        var convertedTransactionAmount = await _currencyRateConversionService.ConvertCurrencyRate(
            amount,
            fromAccount.Currency,
            toAccount.Currency);
        var finalAmount = amount + commission;
        
        ValidateTransactionAndThrow(finalAmount, fromAccount, toAccount);

        fromAccount.Balance -= finalAmount;
        toAccount.Balance += convertedTransactionAmount;

        await _accountRepository.Update(fromAccount, cancellationToken);
        await _accountRepository.Update(toAccount, cancellationToken);

        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            Amount = amount,
            Commission = commission,
            Currency = fromAccount.Currency,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId
        };

        await _transactionRepository.Create(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return transactionId;
    }
}
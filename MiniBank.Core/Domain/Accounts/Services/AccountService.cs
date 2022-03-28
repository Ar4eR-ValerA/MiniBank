using FluentValidation;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Transactions;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Domain.Accounts.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrencyRateConversionService _currencyRateConversionService;
    private readonly IValidator<Account> _accountValidator;
    private readonly IValidator<Transaction> _transactionValidator;

    public AccountService(
        IAccountRepository accountRepository,
        ICurrencyRateConversionService currencyRateConversionService,
        ITransactionRepository transactionRepository, 
        IValidator<Account> accountValidator, 
        IValidator<Transaction> transactionValidator)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _accountValidator = accountValidator;
        _transactionValidator = transactionValidator;
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
    
    public Account GetById(Guid id)
    {
        return _accountRepository.GetById(id);
    }

    public IEnumerable<Account> GetAll()
    {
        return _accountRepository.GetAll();
    }

    public Guid Create(Account account)
    {
        _accountValidator.ValidateAndThrow(account);
        
        account.Id = Guid.NewGuid();
        account.DateOpened = DateTime.Now;
        account.IsActive = true;

        return _accountRepository.Create(account);
    }

    public void Close(Guid id)
    {
        var account = _accountRepository.GetById(id);
        
        if (!account.IsActive)
        {
            throw new UserFriendlyException("Account is already closed");
        }

        if (account.Balance != 0)
        {
            throw new UserFriendlyException("You can't close account with no zero balance");
        }

        account.IsActive = false;
        account.DateClosed = DateTime.Now;

        _accountRepository.Update(account);
    }

    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        if (amount <= 0)
        {
            throw new UserFriendlyException("Amount must be positive");
        }

        var fromAccount = _accountRepository.GetById(fromAccountId);
        var toAccount = _accountRepository.GetById(toAccountId);

        if (fromAccount.UserId == toAccount.UserId)
        {
            return 0;
        }

        double commission = Math.Round(amount * 0.02, 2);

        return commission;
    }

    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        var fromAccount = _accountRepository.GetById(fromAccountId);
        var toAccount = _accountRepository.GetById(toAccountId);

        double commission = CalculateCommission(amount, fromAccount, toAccount);
        double transactionAmount = amount - commission;
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = transactionAmount,
            Commission = commission,
            Currency = fromAccount.Currency,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId
        };
        
        _transactionValidator.ValidateAndThrow(transaction);
        
        double convertedTransactionAmount = _currencyRateConversionService.ConvertCurrencyRate(
            transactionAmount,
            fromAccount.Currency,
            toAccount.Currency);

        fromAccount.Balance -= amount;
        toAccount.Balance += convertedTransactionAmount;

        _accountRepository.Update(fromAccount);
        _accountRepository.Update(toAccount);
        
        return _transactionRepository.Create(transaction);
    }
}
using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrencyRateConversionService _currencyRateConversionService;

    public AccountService(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        ICurrencyRateConversionService currencyRateConversionService,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _currencyRateConversionService = currencyRateConversionService;
    }

    private void ThrowIfTransactionInvalid(double amount, Account fromAccount, Account toAccount)
    {
        if (fromAccount.Id == toAccount.Id)
        {
            throw new ValidationException("Accounts must be different");
        }

        if (amount < 0)
        {
            throw new ValidationException("Transaction amount must be positive");
        }
        
        if (!fromAccount.IsActive)
        {
            throw new ValidationException("Sender is not active");
        }
        
        if (!toAccount.IsActive)
        {
            throw new ValidationException("Receiver is not active");
        }

        if (fromAccount.Balance - amount < 0)
        {
            throw new ValidationException("Balance will be negative after operation");
        }
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
        _userRepository.GetById(account.UserId);
        
        if (account.Currency is not ("RUB" or "EUR" or "USD"))
        {
            throw new ValidationException("Currency of account must be RUB, EUR or USD");
        }

        if (account.Balance < 0)
        {
            throw new ValidationException("Balance of account can't be negative");
        }
        
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
            throw new ValidationException("Account is already closed");
        }

        if (account.Balance != 0)
        {
            throw new ValidationException("You can't close account with no zero balance");
        }

        account.IsActive = false;
        account.DateClosed = DateTime.Now;

        _accountRepository.Update(account);
    }

    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Amount must be positive");
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

        ThrowIfTransactionInvalid(amount, fromAccount, toAccount);

        double commission = CalculateCommission(amount, fromAccountId, toAccountId);
        double transactionAmount = amount - commission;
        double convertedTransactionAmount = _currencyRateConversionService.ConvertCurrencyRate(
            transactionAmount,
            fromAccount.Currency,
            toAccount.Currency);

        fromAccount.Balance -= amount;
        toAccount.Balance += convertedTransactionAmount;

        _accountRepository.Update(fromAccount);
        _accountRepository.Update(toAccount);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = transactionAmount,
            Commission = commission,
            Currency = fromAccount.Currency,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId
        };
        return _transactionRepository.Create(transaction);
    }
}
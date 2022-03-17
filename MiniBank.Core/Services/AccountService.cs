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

    public Account GetById(Guid id)
    {
        return _accountRepository.GetAccountById(id);
    }

    public IEnumerable<Account> GetAll()
    {
        return _accountRepository.GetAllAccounts();
    }

    public Guid CreateAccount(Account account)
    {
        var user = _userRepository.GetUserById(account.UserId);

        if (account.Currency is not ("RUB" or "EUR" or "USD"))
        {
            throw new ValidationException("You can use only RUB, EUR or USD");
        }
        
        user.IncrementAccountsAmount();
        _userRepository.UpdateUser(user);

        return _accountRepository.CreateAccount(account);
    }

    public void CloseAccount(Guid id)
    {
        var account = _accountRepository.GetAccountById(id);

        if (!account.IsActive)
        {
            throw new ValidationException("Account already closed");
        }

        if (account.Balance != 0)
        {
            throw new ValidationException("You can't close account with no zero balance");
        }

        account.DisableAccount(DateTime.Now);
        var user = _userRepository.GetUserById(account.UserId);
        user.DecrementAccountsAmount();

        _userRepository.UpdateUser(user);
        _accountRepository.UpdateAccount(account);
    }

    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Amount must be positive");
        }
        
        var fromAccount = _accountRepository.GetAccountById(fromAccountId);
        var toAccount = _accountRepository.GetAccountById(toAccountId);

        if (fromAccount.UserId == toAccount.UserId)
        {
            return 0;
        }

        double commission = Math.Round(amount * 0.02, 2);

        return commission;
    }

    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        // TODO: мб убрать эти проверки и просто сделать у транзакции проверку на то, что сумма не 0, и id разные?
        // TODO: мб убрать проверки из сущностей и вернуть их в сервис? ‾\_(o.o)_/‾
        // TODO: мб выкидывать в сущностях обычные эксепшены, а в сервисе их ловить и выкидывать user-friendly
        if (amount <= 0)
        {
            throw new ValidationException("Amount must be positive");
        }
        
        if (fromAccountId == toAccountId)
        {
            throw new ValidationException("Accounts must be different");
        }

        var fromAccount = _accountRepository.GetAccountById(fromAccountId);
        var toAccount = _accountRepository.GetAccountById(toAccountId);

        double commission = CalculateCommission(amount, fromAccountId, toAccountId);
        double finalAmount = amount - commission;
        double convertedFinalAmount = _currencyRateConversionService.ConvertCurrencyRate(
            finalAmount,
            fromAccount.Currency,
            toAccount.Currency);

        fromAccount.Balance -= finalAmount;
        toAccount.Balance += convertedFinalAmount;

        _accountRepository.UpdateAccount(fromAccount);
        _accountRepository.UpdateAccount(toAccount);

        var transaction = new Transaction
        {
            Amount = finalAmount,
            Currency = fromAccount.Currency,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId
        };
        return _transactionRepository.CreateTransaction(transaction);
    }
}
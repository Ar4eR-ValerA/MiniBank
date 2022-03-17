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

        user.IncreaseAccountsAmount();
        _userRepository.UpdateUser(user);

        return _accountRepository.CreateAccount(account);
    }

    public void CloseAccount(Guid id)
    {
        var account = _accountRepository.GetAccountById(id);

        account.DisableAccount();
        var user = _userRepository.GetUserById(account.UserId);
        user.DecreaseAccountsAmount();

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

        var transaction = new Transaction(finalAmount, fromAccount.Currency, fromAccountId, toAccountId);
        return _transactionRepository.CreateTransaction(transaction);
    }
}
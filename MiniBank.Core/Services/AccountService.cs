using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRateConversionService _currencyRateConversionService;

    public AccountService(
        IAccountRepository accountRepository,
        IUserRepository userRepository,
        ICurrencyRateConversionService currencyRateConversionService)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
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
        if (!_userRepository.Contains(account.UserId))
        {
            throw new NotFoundException("There is no such user");
        }

        if (account.Currency is not ("RUB" or "EUR" or "USD"))
        {
            throw new ValidationException("You can use only RUB, EUR or USD");
        }

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

        account.IsActive = false;
        account.DateClosed = DateTime.Now;

        _accountRepository.UpdateAccount(account);
    }

    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
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

        if (!fromAccount.IsActive)
        {
            throw new ValidationException("Sender account is not active");
        }

        if (!toAccount.IsActive)
        {
            throw new ValidationException("Receiver account is not active");
        }

        double commission = CalculateCommission(amount, fromAccountId, toAccountId);

        // TODO: Переделать, чтобы смотрелось нормально
        
        fromAccount.Balance -= amount - commission;
        toAccount.Balance += _currencyRateConversionService.ConvertCurrencyRate(
                amount - commission, 
                fromAccount.Currency,
                toAccount.Currency);
        
        _accountRepository.UpdateAccount(fromAccount);
        _accountRepository.UpdateAccount(toAccount);
        
        // TODO: Создавать транзакцию через репозиторий транзакций
        return Guid.Empty;
    }
}
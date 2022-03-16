using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;

namespace MiniBank.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;

    public AccountService(IAccountRepository accountRepository, IUserRepository userRepository)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
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
        throw new NotImplementedException();
    }

    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        throw new NotImplementedException();
    }
}
using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Repositories.DbModels;

namespace MiniBank.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private static readonly List<AccountDbModel> Accounts = new();

    public Account GetAccountById(Guid id)
    {
        var accountDbModel = Accounts.FirstOrDefault(a => a.Id == id);

        if (accountDbModel is null)
        {
            throw new NotFoundException("There is no account with such id");
        }

        return new Account
        {
            Id = accountDbModel.Id,
            UserId = accountDbModel.UserId,
            Balance = accountDbModel.Balance,
            Currency = accountDbModel.Currency,
            IsActive = accountDbModel.IsActive,
            DateOpened = accountDbModel.DateOpened,
            DateClosed = accountDbModel.DateClosed
        };
    }

    public IEnumerable<Account> GetAllAccounts()
    {
        return Accounts.Select(a => new Account
        {
            Id = a.Id,
            UserId = a.UserId,
            Balance = a.Balance,
            Currency = a.Currency,
            IsActive = a.IsActive,
            DateOpened = a.DateOpened,
            DateClosed = a.DateClosed
        });
    }

    public bool Contains(Guid id)
    {
        var account = Accounts.FirstOrDefault(a => a.Id == id);

        return account is not null;
    }

    public Guid CreateAccount(Account account)
    {
        var accountDbModel = new AccountDbModel
        {
            Id = Guid.NewGuid(),
            UserId = account.UserId,
            Balance = account.Balance,
            Currency = account.Currency,
            IsActive = true,
            DateOpened = DateTime.Now,
            DateClosed = account.DateClosed
        };

        Accounts.Add(accountDbModel);

        return accountDbModel.Id;
    }

    public void UpdateAccount(Account account)
    {
        var accountDbModel = Accounts.FirstOrDefault(a => a.Id == account.Id);

        if (accountDbModel is null)
        {
            throw new NotFoundException("There is no such account");
        }

        accountDbModel.Balance = account.Balance;
        accountDbModel.Currency = account.Currency;
        accountDbModel.DateClosed = account.DateClosed;
        accountDbModel.DateOpened = account.DateOpened;
        accountDbModel.IsActive = account.IsActive;
        accountDbModel.UserId = account.UserId;
    }

    public void DeleteAccount(Guid id)
    {
        var accountDbModel = Accounts.FirstOrDefault(a => a.Id == id);

        if (accountDbModel is null)
        {
            throw new NotFoundException("There is no such account");
        }

        Accounts.Remove(accountDbModel);
    }
}
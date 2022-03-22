using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Data.Accounts.Repositories;

public class AccountRepository : IAccountRepository
{
    private static readonly List<AccountDbModel> Accounts = new();

    private AccountDbModel GetAccountDbModelById(Guid id)
    {
        return Accounts.FirstOrDefault(a => a.Id == id);
    }

    public Account GetById(Guid id)
    {
        var accountDbModel = GetAccountDbModelById(id);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {id}");
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

    public IEnumerable<Account> GetAll()
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

    public Guid Create(Account account)
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

    public void Update(Account account)
    {
        var accountDbModel = GetAccountDbModelById(account.Id);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {account.Id}");
        }

        accountDbModel.Balance = account.Balance;
        accountDbModel.Currency = account.Currency;
        accountDbModel.DateClosed = account.DateClosed;
        accountDbModel.DateOpened = account.DateOpened;
        accountDbModel.IsActive = account.IsActive;
        accountDbModel.UserId = account.UserId;
    }

    public void Delete(Guid id)
    {
        var accountDbModel = GetAccountDbModelById(id);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {id}");
        }

        Accounts.Remove(accountDbModel);
    }

    public bool HasUserLinkedAccounts(Guid usedId)
    {
        var accountDbModel = Accounts.FirstOrDefault(a => a.UserId == usedId);

        return accountDbModel is not null;
    }
}
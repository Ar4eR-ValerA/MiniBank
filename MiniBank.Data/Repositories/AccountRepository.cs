using MiniBank.Core.Entities;
using MiniBank.Core.Repositories;
using MiniBank.Data.Repositories.DbModels;

namespace MiniBank.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private List<AccountDbModel> _accounts;

    public AccountRepository()
    {
        _accounts = new List<AccountDbModel>();
    }

    public Account GetAccountById(Guid id)
    {
        var accountDbModel = _accounts.FirstOrDefault(a => a.Id == id);

        if (accountDbModel is null)
        {
            throw new Exception("There is no account with such id");
        }

        return new Account(
            accountDbModel.Id,
            accountDbModel.UserId,
            accountDbModel.Balance,
            accountDbModel.Currency,
            accountDbModel.IsActive,
            accountDbModel.DateOpened,
            accountDbModel.DateClosed);
    }

    public IEnumerable<Account> GetAllAccounts()
    {
        return _accounts.Select(a => new Account(
            a.Id,
            a.UserId,
            a.Balance,
            a.Currency,
            a.IsActive,
            a.DateOpened,
            a.DateClosed));
    }

    public Guid CreateAccount(Account account)
    {
        var accountDbModel = new AccountDbModel(
            Guid.NewGuid(),
            account.UserId,
            account.Balance,
            account.Currency,
            account.IsActive,
            account.DateOpened,
            account.DateClosed);

        _accounts.Add(accountDbModel);

        return accountDbModel.Id;
    }

    public void UpdateAccount(Account account)
    {
        var accountDbModel = _accounts.FirstOrDefault(a => a.Id == account.Id);

        if (accountDbModel is null)
        {
            throw new Exception("There is no such account");
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
        var accountDbModel = _accounts.FirstOrDefault(a => a.Id == id);

        if (accountDbModel is null)
        {
            throw new Exception("There is no such account");
        }

        _accounts.Remove(accountDbModel);
    }
}
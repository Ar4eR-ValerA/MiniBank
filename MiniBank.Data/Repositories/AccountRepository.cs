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
        return Accounts.Select(a => new Account(
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
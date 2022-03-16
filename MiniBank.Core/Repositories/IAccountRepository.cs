using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IAccountRepository
{
    Account GetAccountById(Guid id);
    IEnumerable<Account> GetAllAccounts();
    bool Contains(Guid id);
    Guid CreateAccount(Account account);
    void UpdateAccount(Account account);
    void DeleteAccount(Guid id);
}
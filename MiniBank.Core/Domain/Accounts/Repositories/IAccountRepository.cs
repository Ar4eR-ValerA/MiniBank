namespace MiniBank.Core.Domain.Accounts.Repositories;

public interface IAccountRepository
{
    Account GetById(Guid id);
    IEnumerable<Account> GetAll();
    Guid Create(Account account);
    void Update(Account account);
    void Delete(Guid id);
    bool HasUserLinkedAccounts(Guid usedId);
}
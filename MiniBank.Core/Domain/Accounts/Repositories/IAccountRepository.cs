namespace MiniBank.Core.Domain.Accounts.Repositories;

public interface IAccountRepository
{
    Task<Account> GetById(Guid id);
    Task<IEnumerable<Account>> GetAll();
    Task Create(Account account);
    Task Update(Account account);
    Task Delete(Guid id);
    Task<bool> HasUserLinkedAccounts(Guid usedId);
}
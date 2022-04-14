namespace MiniBank.Core.Domain.Accounts.Repositories;

public interface IAccountRepository
{
    Task<Account> GetById(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken);
    Task Create(Account account, CancellationToken cancellationToken);
    Task Update(Account account, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<bool> HasUserLinkedAccounts(Guid usedId, CancellationToken cancellationToken);
}
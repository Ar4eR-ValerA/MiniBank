namespace MiniBank.Core.Domain.Transactions.Repositories;

public interface ITransactionRepository
{
    
    Task<Transaction> GetById(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAll(CancellationToken cancellationToken);
    Task Create(Transaction transaction, CancellationToken cancellationToken);
    Task Update(Transaction transaction, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}
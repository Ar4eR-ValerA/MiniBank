namespace MiniBank.Core.Domain.Transactions.Repositories;

public interface ITransactionRepository
{
    
    Task<Transaction> GetById(Guid id);
    Task<IEnumerable<Transaction>> GetAll();
    Task Create(Transaction transaction);
    Task Update(Transaction transaction);
    Task Delete(Guid id);
}
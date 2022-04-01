namespace MiniBank.Core.Domain.Transactions.Repositories;

public interface ITransactionRepository
{
    
    Transaction GetById(Guid id);
    IEnumerable<Transaction> GetAll();
    void Create(Transaction transaction);
    void Update(Transaction transaction);
    void Delete(Guid id);
}
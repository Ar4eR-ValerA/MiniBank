using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface ITransactionRepository
{
    Transaction GetTransactionById(Guid id);
    IEnumerable<Transaction> GetAllTransactions();
    bool Contains(Guid id);
    Guid CreateTransaction(Transaction transaction);
    void UpdateTransaction(Transaction transaction);
    void DeleteTransaction(Guid id);
}
using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface ITransactionRepository
{
    Transaction GetTransactionById(Guid id);
    IEnumerable<Transaction> GetAllTransactions();
    Guid CreateTransaction(Transaction transaction);
    void UpdateTransaction(Transaction transaction);
    void DeleteTransaction(Guid id);
}
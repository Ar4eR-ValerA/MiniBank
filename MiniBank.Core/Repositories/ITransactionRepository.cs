using System.Transactions;

namespace MiniBank.Core.Repositories;

public interface ITransactionRepository
{
    Transaction GetById(Guid id);
    IEnumerable<Transaction> GetAll();
    Guid CreateTransaction(Transaction transaction);
}
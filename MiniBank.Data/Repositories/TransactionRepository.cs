using System.Transactions;
using MiniBank.Core.Repositories;

namespace MiniBank.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    public Transaction GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Transaction> GetAll()
    {
        throw new NotImplementedException();
    }

    public Guid CreateTransaction(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}
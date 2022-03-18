using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface ITransactionRepository
{
    Transaction GetById(Guid id);
    IEnumerable<Transaction> GetAll();
    Guid Create(Transaction transaction);
    void Update(Transaction transaction);
    void Delete(Guid id);
}
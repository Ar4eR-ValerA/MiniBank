using MiniBank.Core.Domain.Transactions;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Tools;

namespace MiniBank.Data.Transactions.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private static readonly List<TransactionDbModel> Transactions = new();

    private TransactionDbModel GetTransactionDbModelById(Guid id)
    {
        return Transactions.FirstOrDefault(t => t.Id == id);
    }

    public Transaction GetById(Guid id)
    {
        var transactionDbModel = GetTransactionDbModelById(id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {id}");
        }

        return new Transaction
        {
            Id = transactionDbModel.Id,
            Amount = transactionDbModel.Amount,
            Commission = transactionDbModel.Commission,
            Currency = transactionDbModel.Currency,
            FromAccountId = transactionDbModel.FromAccountId,
            ToAccountId = transactionDbModel.ToAccountId
        };
    }

    public IEnumerable<Transaction> GetAll()
    {
        return Transactions.Select(t => new Transaction
        {
            Id = t.Id,
            Amount = t.Amount,
            Commission = t.Commission,
            Currency = t.Currency,
            FromAccountId = t.FromAccountId,
            ToAccountId = t.ToAccountId
        });
    }

    public Guid Create(Transaction transaction)
    {
        var transactionDbModel = new TransactionDbModel
        {
            Id = Guid.NewGuid(),
            Amount = transaction.Amount,
            Commission = transaction.Commission,
            Currency = transaction.Currency,
            FromAccountId = transaction.FromAccountId,
            ToAccountId = transaction.ToAccountId
        };

        Transactions.Add(transactionDbModel);

        return transactionDbModel.Id;
    }

    public void Update(Transaction transaction)
    {
        var transactionDbModel = GetTransactionDbModelById(transaction.Id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {transaction.Id}");
        }

        transactionDbModel.Amount = transaction.Amount;
        transactionDbModel.Commission = transactionDbModel.Commission;
        transactionDbModel.Currency = transaction.Currency;
        transactionDbModel.FromAccountId = transaction.FromAccountId;
        transactionDbModel.ToAccountId = transaction.ToAccountId;
    }

    public void Delete(Guid id)
    {
        var transactionDbModel = GetTransactionDbModelById(id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {id}");
        }

        Transactions.Remove(transactionDbModel);
    }
}
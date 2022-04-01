using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Domain.Transactions;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Contexts;

namespace MiniBank.Data.Transactions.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly MiniBankContext _context;

    public TransactionRepository(MiniBankContext context)
    {
        _context = context;
    }
    public Transaction GetById(Guid id)
    {
        var transactionDbModel = _context.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == id);

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
        return _context.Transactions.AsNoTracking().Select(t => new Transaction
        {
            Id = t.Id,
            Amount = t.Amount,
            Commission = t.Commission,
            Currency = t.Currency,
            FromAccountId = t.FromAccountId,
            ToAccountId = t.ToAccountId
        });
    }

    public void Create(Transaction transaction)
    {
        var transactionDbModel = new TransactionDbModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Commission = transaction.Commission,
            Currency = transaction.Currency,
            FromAccountId = transaction.FromAccountId,
            ToAccountId = transaction.ToAccountId
        };

        _context.Transactions.Add(transactionDbModel);
    }

    public void Update(Transaction transaction)
    {
        var transactionDbModel = _context.Transactions.FirstOrDefault(t => t.Id == transaction.Id);

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
        var transactionDbModel = _context.Transactions.FirstOrDefault(t => t.Id == id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {id}");
        }

        _context.Transactions.Remove(transactionDbModel);
    }
}
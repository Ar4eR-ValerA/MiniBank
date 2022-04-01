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

    public async Task<Transaction> GetById(Guid id)
    {
        var transactionDbModel = await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

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

    public async Task<IEnumerable<Transaction>> GetAll()
    {
        return await _context.Transactions.AsNoTracking().Select(t => new Transaction
        {
            Id = t.Id,
            Amount = t.Amount,
            Commission = t.Commission,
            Currency = t.Currency,
            FromAccountId = t.FromAccountId,
            ToAccountId = t.ToAccountId
        }).ToListAsync();
    }

    public async Task Create(Transaction transaction)
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

        await _context.Transactions.AddAsync(transactionDbModel);
    }

    public async Task Update(Transaction transaction)
    {
        var transactionDbModel = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {transaction.Id}");
        }

        transactionDbModel.Amount = transaction.Amount;
        transactionDbModel.Commission = transactionDbModel.Commission;
        transactionDbModel.Currency = transaction.Currency;
        transactionDbModel.FromAccountId = transaction.FromAccountId;
        transactionDbModel.ToAccountId = transaction.ToAccountId;

        _context.Transactions.Update(transactionDbModel);
    }

    public async Task Delete(Guid id)
    {
        var transactionDbModel = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

        if (transactionDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no transaction with such id: {id}");
        }

        _context.Transactions.Remove(transactionDbModel);
    }
}
using Microsoft.EntityFrameworkCore;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Tools;
using MiniBank.Data.Contexts;

namespace MiniBank.Data.Accounts.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly MiniBankContext _context;

    public AccountRepository(MiniBankContext context)
    {
        _context = context;
    }

    public async Task<Account> GetById(Guid id, CancellationToken cancellationToken)
    {
        var accountDbModel = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {id}");
        }

        return new Account
        {
            Id = accountDbModel.Id,
            UserId = accountDbModel.UserId,
            Balance = accountDbModel.Balance,
            Currency = accountDbModel.Currency,
            IsActive = accountDbModel.IsActive,
            DateOpened = accountDbModel.DateOpened,
            DateClosed = accountDbModel.DateClosed
        };
    }

    public async Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Accounts.AsNoTracking().Select(a => new Account
        {
            Id = a.Id,
            UserId = a.UserId,
            Balance = a.Balance,
            Currency = a.Currency,
            IsActive = a.IsActive,
            DateOpened = a.DateOpened,
            DateClosed = a.DateClosed
        }).ToListAsync(cancellationToken);
    }

    public async Task Create(Account account, CancellationToken cancellationToken)
    {
        var accountDbModel = new AccountDbModel
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            DateOpened = account.DateOpened,
            DateClosed = account.DateClosed
        };

        await _context.Accounts.AddAsync(accountDbModel, cancellationToken);
    }

    public async Task Update(Account account, CancellationToken cancellationToken)
    {
        var accountDbModel = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == account.Id, cancellationToken);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {account.Id}");
        }

        accountDbModel.Balance = account.Balance;
        accountDbModel.Currency = account.Currency;
        accountDbModel.DateClosed = account.DateClosed;
        accountDbModel.DateOpened = account.DateOpened;
        accountDbModel.IsActive = account.IsActive;
        accountDbModel.UserId = account.UserId;

        _context.Accounts.Update(accountDbModel);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var accountDbModel = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (accountDbModel is null)
        {
            throw new ObjectNotFoundException($"There is no account with such id: {id}");
        }

        _context.Accounts.Remove(accountDbModel);
    }

    public async Task<bool> HasUserLinkedAccounts(Guid usedId, CancellationToken cancellationToken)
    {
        var accountDbModel = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == usedId, cancellationToken);

        return accountDbModel is not null;
    }
}
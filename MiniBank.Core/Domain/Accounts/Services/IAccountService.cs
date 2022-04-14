namespace MiniBank.Core.Domain.Accounts.Services;

public interface IAccountService
{
    Task<Account> GetById(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken);
    Task<Guid> Create(Account account, CancellationToken cancellationToken);
    Task Close(Guid id, CancellationToken cancellationToken);

    Task<double> CalculateCommission(
        double amount,
        Guid fromAccountId,
        Guid toAccountId,
        CancellationToken cancellationToken);

    Task<Guid> MakeTransaction(
        double amount,
        Guid fromAccountId,
        Guid toAccountId,
        CancellationToken cancellationToken);
}
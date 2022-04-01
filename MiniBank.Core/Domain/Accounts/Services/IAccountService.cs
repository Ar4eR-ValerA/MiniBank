namespace MiniBank.Core.Domain.Accounts.Services;

public interface IAccountService
{
    Task<Account> GetById(Guid id);
    Task<IReadOnlyList<Account>> GetAll();
    Task<Guid> Create(Account account);
    Task Close(Guid id);
    Task<double> CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId);
    Task<Guid> MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId);
}
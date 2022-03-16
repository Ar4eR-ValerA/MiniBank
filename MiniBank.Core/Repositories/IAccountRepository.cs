using MiniBank.Core.Entities;

namespace MiniBank.Core.Repositories;

public interface IAccountRepository
{
    Account GetById(Guid id);
    IEnumerable<Account> GetAll();
    Guid CreateAccount(Account account);
    void CloseAccount(Guid id);
    double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId);
    Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId);
}
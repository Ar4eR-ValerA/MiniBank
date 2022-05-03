using MiniBank.Core.Domain.Currencies;

namespace MiniBank.Core.Domain.Transactions;

public class Transaction
{
    public Guid Id { get; set; }
    public Currency Currency { get; set; }
    public double Amount { get; set; }
    public double Commission { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}
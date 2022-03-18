using MiniBank.Core.Tools;

namespace MiniBank.Core.Entities;

public class Transaction
{
    public Transaction(double amount, double commission, string currency, Guid fromAccountId, Guid toAccountId)
    {
        Id = Guid.NewGuid();

        if (amount <= 0)
        {
            throw new ValidationException("Transaction amount must be positive");
        }

        Amount = amount;
        Commission = commission;
        Currency = currency;

        if (fromAccountId == toAccountId)
        {
            throw new ValidationException("Accounts must be different");
        }

        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
    }

    public Transaction(Guid id, double amount, double commission, string currency, Guid fromAccountId, Guid toAccountId)
    {
        Id = id;
        Amount = amount;
        Commission = commission;
        Currency = currency;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
    }

    public Guid Id { get; }
    public double Amount { get; }
    public double Commission { get; }
    public string Currency { get; }
    public Guid FromAccountId { get; }
    public Guid ToAccountId { get; }
}
namespace MiniBank.Core.Entities;

public class Transaction
{
    public Transaction(double amount, string currency, Guid fromAccountId, Guid toAccountId)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Currency = currency;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
    }

    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}
namespace MiniBank.Core.Entities;

public class Transaction
{
    public Guid Id { get; init; }
    public double Amount { get; init; }
    public string Currency { get; init; }
    public Guid FromAccountId { get; init; }
    public Guid ToAccountId { get; init; }
}
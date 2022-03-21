namespace MiniBank.Data.Transactions;

public class TransactionDbModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public double Commission { get; set; }
    public string Currency { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}
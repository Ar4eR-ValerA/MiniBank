namespace MiniBank.Data.Repositories.DbModels;

public class TransactionDbModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}
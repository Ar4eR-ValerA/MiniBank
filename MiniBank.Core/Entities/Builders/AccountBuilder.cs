namespace MiniBank.Core.Entities.Builders;

public class AccountBuilder
{
    public AccountBuilder(
        Guid userId, 
        double balance, 
        string currency,
        DateTime dateClosed)
    {
        UserId = userId;
        Balance = balance;
        Currency = currency ?? throw new Exception("Currency is null");
        DateClosed = dateClosed;
    }
    
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public DateTime DateClosed { get; set; }

    public Account Build()
    {
        return new Account(
            Guid.Empty,
            UserId,
            Balance,
            Currency,
            true,
            DateTime.Now,
            DateClosed);
    }
}
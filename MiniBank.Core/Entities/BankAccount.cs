namespace MiniBank.Core.Entities;

public class BankAccount
{
    public BankAccount(
        Guid userId, 
        double balance, 
        string currency, 
        bool isActive, 
        DateTime dateOpened,
        DateTime dateClosed)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Balance = balance;
        Currency = currency;
        IsActive = isActive;
        DateOpened = dateOpened;
        DateClosed = dateClosed;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateOpened { get; set; }
    public DateTime DateClosed { get; set; }
}
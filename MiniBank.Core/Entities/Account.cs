using MiniBank.Core.Tools;

namespace MiniBank.Core.Entities;

public class Account
{
    private double _balance;

    public Account(Guid userId, double balance, string currency)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        _balance = balance;

        if (currency is not ("RUB" or "EUR" or "USD"))
        {
            throw new ValidationException("Currency of account must be RUB, EUR or USD");
        }

        Currency = currency;
        IsActive = true;
        DateOpened = DateTime.Now;
    }

    public Account(
        Guid id,
        Guid userId,
        double balance,
        string currency,
        bool isActive,
        DateTime dateOpened,
        DateTime? dateClosed)
    {
        Id = id;
        UserId = userId;
        _balance = balance;
        Currency = currency;
        IsActive = isActive;
        DateOpened = dateOpened;
        DateClosed = dateClosed;
    }

    public Guid Id { get; }
    public Guid UserId { get; }

    public double Balance
    {
        get => _balance;
        set
        {
            if (!IsActive)
            {
                throw new ValidationException($"Account {Id} is closed");
            }

            if (value < 0)
            {
                throw new ValidationException("Balance will be negative after operation");
            }

            _balance = value;
        }
    }

    public string Currency { get; }
    public bool IsActive { get; private set; }
    public DateTime DateOpened { get; }
    public DateTime? DateClosed { get; private set; }

    public void DisableAccount()
    {
        if (!IsActive)
        {
            throw new ValidationException("Account is already closed");
        }
        
        if (_balance != 0)
        {
            throw new ValidationException("You can't close account with no zero balance");
        }

        IsActive = false;
        DateClosed = DateTime.Now;
    }
}
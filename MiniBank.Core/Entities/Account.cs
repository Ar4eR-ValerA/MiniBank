using MiniBank.Core.Tools;

namespace MiniBank.Core.Entities;

public class Account
{
    private double _balance;
    private bool _isActive;
    private DateTime _dateClosed;
    private readonly string _currency;

    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public double Balance
    {
        get => _balance;
        set
        {
            if (!IsActive)
            {
                throw new ValidationException($"Account {Id} is closed");
            }

            if (_balance - value < 0)
            {
                throw new ValidationException("After operation balance will be negative");
            }

            _balance = value;
        }
    }

    public string Currency
    {
        get => _currency;
        init
        {
            if (value is not ("RUB" or "EUR" or "USD"))
            {
                throw new ValidationException("Currency of account must be RUB, EUR or USD");
            }

            _currency = value;
        }
    }

    public bool IsActive
    {
        get => _isActive;
        init
        {
            if (value == false && _balance != 0)
            {
                throw new ValidationException("You can't close account with no zero balance");
            }

            _isActive = value;
        }
    }

    public DateTime DateOpened { get; init; }

    public DateTime DateClosed
    {
        get => _dateClosed;
        init => _dateClosed = value;
    }

    public void DisableAccount(DateTime dateClosed)
    {
        _isActive = false;
        _dateClosed = dateClosed;
    }
}
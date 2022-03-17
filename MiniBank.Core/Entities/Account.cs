using MiniBank.Core.Tools;

namespace MiniBank.Core.Entities;

public class Account
{
    private double _balance;
    private bool _isActive;
    private DateTime _dateClosed;

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

    public string Currency { get; init; }

    public bool IsActive
    {
        get => _isActive;
        init => _isActive = value;
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
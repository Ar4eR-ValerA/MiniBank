using MiniBank.Core.Tools;

namespace MiniBank.Core.Entities;

public class Transaction
{
    private readonly Guid _fromAccountId;
    private readonly Guid _toAccountId;
    private readonly double _amount;
    public Guid Id { get; init; }

    public double Amount
    {
        get => _amount;
        init
        {
            if (_amount <= 0)
            {
                throw new ValidationException("Transaction amount must be positive");
            }

            _amount = value;
        }
    }

    public string Currency { get; init; }

    public Guid FromAccountId
    {
        get => _fromAccountId;
        init
        {
            if (value == ToAccountId)
            {
                throw new ValidationException("Accounts must be different");
            }

            _fromAccountId = value;
        }
    }

    public Guid ToAccountId
    {
        get => _toAccountId;
        init
        {
            if (value == FromAccountId)
            {
                throw new ValidationException("Accounts must be different");
            }

            _toAccountId = value;
        }
    }
}
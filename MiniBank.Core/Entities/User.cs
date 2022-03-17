namespace MiniBank.Core.Entities;

public class User
{
    private int _accountsAmount;
    public Guid Id { get; init; }
    public string Login { get; set; }
    public string Email { get; set; }

    public int AccountsAmount
    {
        get => _accountsAmount;
        init
        {
            if (value < 0)
            {
                throw new Exception("Accounts amount can't be negative");
            }
            _accountsAmount = value;
        }
    }

    public void IncrementAccountsAmount()
    {
        _accountsAmount++;
    }
    
    public void DecrementAccountsAmount()
    {
        if (_accountsAmount == 0)
        {
            throw new Exception("Accounts amount can't be negative");
        }
        
        _accountsAmount--;
    }
}
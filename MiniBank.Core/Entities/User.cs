namespace MiniBank.Core.Entities;

public class User
{
    public User(string login, string email)
    {
        Id = Guid.NewGuid();
        Login = login;
        Email = email;
        AccountsAmount = 0;
    }

    public User(Guid id, string login, string email, int accountsAmount)
    {
        Id = id;
        Login = login;
        Email = email;
        AccountsAmount = accountsAmount;
    }

    public Guid Id { get; }
    public string Login { get; set; }
    public string Email { get; set; }
    public int AccountsAmount { get; private set; }

    public void IncreaseAccountsAmount()
    {
        AccountsAmount++;
    }

    public void DecreaseAccountsAmount()
    {
        if (AccountsAmount == 0)
        {
            throw new Exception("Accounts amount can't be negative");
        }

        AccountsAmount--;
    }
}
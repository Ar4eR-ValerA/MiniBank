namespace MiniBank.Core.Entities;

public class User
{ 
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public int AccountsAmount { get; set; }

    public void IncrementAccountsAmount()
    {
        AccountsAmount++;
    }
    
    public void DecrementAccountsAmount()
    {
        AccountsAmount--;
    }
}
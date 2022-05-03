namespace MiniBank.Core.Domain.Users;

public class User
{ 
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
}
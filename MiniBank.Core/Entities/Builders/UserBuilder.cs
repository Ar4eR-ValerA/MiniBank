namespace MiniBank.Core.Entities.Builders;

public class UserBuilder
{
    public UserBuilder(string login, string email)
    {
        Login = login ?? throw new Exception("Login is null");
        Email = email ?? throw new Exception("Email is null");
    }
    
    public string Login { get; set; }
    public string Email { get; set; }

    public User Build()
    {
        return new User(Guid.Empty, Login, Email, 0);
    }
}
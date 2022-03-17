namespace MiniBank.Core.Entities.Builders;

public class UserBuilder
{
    public UserBuilder(string login, string email)
    {
        Login = login;
        Email = email;
    }
    
    public string Login { get; set; }
    public string Email { get; set; }

    public User Build()
    {
        return new User(Login, Email);
    }
}
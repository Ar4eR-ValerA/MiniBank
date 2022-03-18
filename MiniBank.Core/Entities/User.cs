namespace MiniBank.Core.Entities;

public class User
{
    public User(string login, string email)
    {
        Id = Guid.NewGuid();
        Login = login;
        Email = email;
    }

    public User(Guid id, string login, string email)
    {
        Id = id;
        Login = login;
        Email = email;
    }

    public Guid Id { get; }
    public string Login { get; set; }
    public string Email { get; set; }
}
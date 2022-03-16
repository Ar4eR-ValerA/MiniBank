namespace MiniBank.Core.Entities;

public class User
{
    public User(Guid id, string login, string email)
    {
        Id = id;
        Login = login ?? throw new Exception("Login is null");
        Email = email ?? throw new Exception("Email is null");
    }

    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    //TODO: Сделать счётчик того, сколько есть активных аккаунтов.
}
namespace MiniBank.Web.Dtos;

public class UserDto
{
    public UserDto(Guid id, string login, string email)
    {
        Id = id;
        Login = login ?? throw new Exception("Login is null");
        Email = email ?? throw new Exception("Email is null");
    }

    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
}
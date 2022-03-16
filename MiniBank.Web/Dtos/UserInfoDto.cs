using System.ComponentModel.DataAnnotations;

namespace MiniBank.Web.Dtos;

public class UserInfoDto
{
    private string _login = string.Empty;
    private string _email = string.Empty;

    public string Login
    {
        get => _login;
        set => _login = value ?? throw new ValidationException("Login is null");
    }

    public string Email
    {
        get => _email;
        set => _email = value ?? throw new ValidationException("Email is null");
    }
}
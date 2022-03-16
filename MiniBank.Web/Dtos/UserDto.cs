using MiniBank.Core.Tools;

namespace MiniBank.Web.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public int AccountsAmount { get; set; }
}
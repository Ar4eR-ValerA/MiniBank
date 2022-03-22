using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Domain.Users;
using MiniBank.Core.Domain.Users.Services;
using MiniBank.Web.Controllers.Users.Dto;

namespace MiniBank.Web.Controllers.Users;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public UserDto GetById(Guid id)
    {
        var user = _userService.GetById(id);

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };
    }

    [HttpGet]
    public IEnumerable<UserDto> GetAll()
    {
        var users = _userService.GetAll();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        });
    }

    [HttpPost("create")]
    public Guid Create(UserCreateDto userCreateDto)
    {
        var user = new User
        {
            Login = userCreateDto.Login,
            Email = userCreateDto.Email
        };

        return _userService.Create(user);
    }

    [HttpPut("update/{id:guid}")]
    public void Update(Guid id, UserUpdateDto userUpdateDto)
    {
        var user = new User
        {
            Id = id,
            Login = userUpdateDto.Login,
            Email = userUpdateDto.Email
        };

        _userService.Update(user);
    }

    [HttpDelete("{id:guid}")]
    public void Delete(Guid id)
    {
        _userService.Delete(id);
    }
}
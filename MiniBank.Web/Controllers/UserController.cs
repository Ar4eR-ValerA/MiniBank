using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities;
using MiniBank.Core.Entities.Builders;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Web.Dtos;

namespace MiniBank.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetUserById")]
    public UserDto GetUserById([FromQuery] Guid id)
    {
        var user = _userService.GetById(id);

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            AccountsAmount = user.AccountsAmount
        };
    }

    [HttpGet("GetAllUsers")]
    public IEnumerable<UserDto> GetAllUsers()
    {
        var users = _userService.GetAll();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email,
            AccountsAmount = u.AccountsAmount
        });
    }

    [HttpPost("CreateUser")]
    public Guid CreateUser([FromQuery] UserInfoDto userInfoDto)
    {
        var user = new UserBuilder(userInfoDto.Login, userInfoDto.Email).Build();

        return _userService.CreateUser(user);
    }

    [HttpPut("UpdateUser")]
    public void UpdateUser([FromQuery] Guid id, [FromQuery] UserInfoDto userInfoDto)
    {
        var user = new User(id, userInfoDto.Login, userInfoDto.Email);

        _userService.UpdateUser(user);
    }

    [HttpDelete("DeleteUser")]
    public void DeleteUser([FromQuery] Guid id)
    {
        _userService.DeleteUser(id);
    }
}
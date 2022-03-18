using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities;
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

    [HttpGet("GetById")]
    public UserDto GetById([FromQuery] Guid id)
    {
        var user = _userService.GetById(id);

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };
    }

    [HttpGet("GetAll")]
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

    [HttpPost("Create")]
    public Guid Create([FromQuery] UserInfoDto userInfoDto)
    {
        var user = new User
        {
            Login = userInfoDto.Login,
            Email = userInfoDto.Email
        };

        return _userService.Create(user);
    }

    [HttpPut("Update")]
    public void Update([FromQuery] UserDto userDto)
    {
        var user = new User
        {
            Id = userDto.Id,
            Login = userDto.Login,
            Email = userDto.Email
        };

        _userService.Update(user);
    }

    [HttpDelete("Delete")]
    public void Delete([FromQuery] Guid id)
    {
        _userService.Delete(id);
    }
}
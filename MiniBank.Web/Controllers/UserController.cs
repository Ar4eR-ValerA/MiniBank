using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Web.Dtos;

namespace MiniBank.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetUserById")]
    public UserDto GetUserById([FromQuery] Guid id)
    {
        var user = _userService.GetById(id);

        return new UserDto(user.Id, user.Login, user.Email);
    }
    
    [HttpGet("GetAllUsers")]
    public IEnumerable<UserDto> GetAllUsers()
    {
        var users = _userService.GetAll();

        return users.Select(u => new UserDto(u.Id, u.Login, u.Email));
    }

    [HttpPost("CreateUser")]
    public Guid CreateUser([FromQuery] string login, string email)
    {
        var user = new User(Guid.Empty, login, email);
        
        return _userService.CreateUser(user);
    }

    [HttpPut("UpdateUser")]
    public void UpdateUser([FromQuery] UserDto userDto)
    {
        var user = new User(userDto.Id, userDto.Login, userDto.Email);
        
        _userService.UpdateUser(user);
    }
    
    [HttpDelete("DeleteUser")]
    public void DeleteUser([FromQuery] Guid id)
    {
        _userService.DeleteUser(id);
    }
}
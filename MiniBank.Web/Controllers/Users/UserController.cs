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
    public async Task<UserDto> GetById(Guid id)
    {
        var user = await _userService.GetById(id);

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };
    }

    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> GetAll()
    {
        var users = await _userService.GetAll();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        }).ToList();
    }

    [HttpPost("create")]
    public async Task<Guid> Create(UserCreateDto userCreateDto)
    {
        var user = new User
        {
            Login = userCreateDto.Login,
            Email = userCreateDto.Email
        };

        return await _userService.Create(user);
    }

    [HttpPut("update/{id:guid}")]
    public async Task Update(Guid id, UserUpdateDto userUpdateDto)
    {
        var user = new User
        {
            Id = id,
            Login = userUpdateDto.Login,
            Email = userUpdateDto.Email
        };

        await _userService.Update(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
    {
        await _userService.Delete(id);
    }
}
﻿using Microsoft.AspNetCore.Mvc;
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
    public async Task<UserDto> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetById(id, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email
        };
    }

    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAll(cancellationToken);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email
        }).ToList();
    }

    [HttpPost("create")]
    public async Task<Guid> Create(UserCreateDto userCreateDto, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Login = userCreateDto.Login,
            Email = userCreateDto.Email
        };

        return await _userService.Create(user, cancellationToken);
    }

    [HttpPut("update/{id:guid}")]
    public async Task Update(Guid id, UserUpdateDto userUpdateDto, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Id = id,
            Login = userUpdateDto.Login,
            Email = userUpdateDto.Email
        };

        await _userService.Update(user, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _userService.Delete(id, cancellationToken);
    }
}
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Web.Dtos;

namespace MiniBank.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("GetById")]
    public AccountDto GetById(Guid id)
    {
        var account = _accountService.GetById(id);

        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            DateOpened = account.DateOpened,
            DateClosed = account.DateClosed
        };
    }

    [HttpGet("GetAll")]
    public IEnumerable<AccountDto> GetAll()
    {
        var accounts = _accountService.GetAll();

        return accounts.Select(a => new AccountDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Balance = a.Balance,
            Currency = a.Currency,
            IsActive = a.IsActive,
            DateOpened = a.DateOpened,
            DateClosed = a.DateClosed
        });
    }

    [HttpPost("Create")]
    public Guid Create([FromQuery] AccountInfoDto accountInfoDto)
    {
        var account = new Account
        {
            UserId = accountInfoDto.UserId,
            Balance = accountInfoDto.Balance,
            Currency = accountInfoDto.Currency
        };

        return _accountService.Create(account);
    }

    [HttpPut("Close")]
    public void Close(Guid id)
    {
        _accountService.Close(id);
    }

    [HttpGet("CalculateCommission")]
    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.CalculateCommission(amount, fromAccountId, toAccountId);
    }

    [HttpPost("MakeTransaction")]
    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.MakeTransaction(amount, fromAccountId, toAccountId);
    }
}
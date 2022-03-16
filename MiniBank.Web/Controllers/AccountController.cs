using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities.Builders;
using MiniBank.Core.Services;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Web.Dtos;

namespace MiniBank.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController()
    {
        _accountService = new AccountService();
    }

    [HttpGet("GetAccountById")]
    public AccountDto GetAccountById([FromQuery] Guid id)
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

    [HttpGet("GetAllAccounts")]
    public IEnumerable<AccountDto> GetAllAccounts()
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

    [HttpPost("CreateAccount")]
    public Guid CreateAccount([FromQuery] AccountInfoDto accountInfoDto)
    {
        var account = new AccountBuilder(
                accountInfoDto.UserId,
                accountInfoDto.Balance,
                accountInfoDto.Currency,
                accountInfoDto.DateClosed)
            .Build();

        return _accountService.CreateAccount(account);
    }

    [HttpPut("CloseAccount")]
    public void CloseAccount(Guid id)
    {
        _accountService.CloseAccount(id);
    }

    [HttpGet("CalculateCommission")]
    public double CalculateCommission(
        [FromQuery] double amount, 
        [FromQuery] Guid fromAccountId, 
        [FromQuery] Guid toAccountId)
    {
        return _accountService.CalculateCommission(amount, fromAccountId, toAccountId);
    }

    [HttpPost("MakeTransaction")]
    public Guid MakeTransaction(
        [FromQuery] double amount, 
        [FromQuery] Guid fromAccountId, 
        [FromQuery] Guid toAccountId)
    {
        return _accountService.MakeTransaction(amount, fromAccountId, toAccountId);
    }
}
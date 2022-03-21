using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Web.Controllers.Accounts.Dto;

namespace MiniBank.Web.Controllers.Accounts;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("get/{id:guid}")]
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

    [HttpGet("get")]
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

    [HttpPost("create")]
    public Guid Create(AccountCreateDto accountCreateDto)
    {
        var account = new Account
        {
            UserId = accountCreateDto.UserId,
            Balance = accountCreateDto.Balance,
            Currency = accountCreateDto.Currency
        };

        return _accountService.Create(account);
    }

    [HttpPut("close/{id:guid}")]
    public void Close(Guid id)
    {
        _accountService.Close(id);
    }

    [HttpGet("calculate-commission")]
    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.CalculateCommission(amount, fromAccountId, toAccountId);
    }

    [HttpPost("make-transaction")]
    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.MakeTransaction(amount, fromAccountId, toAccountId);
    }
}